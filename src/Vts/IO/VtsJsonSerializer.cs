using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.IO
{
    /// <summary>
    /// json serializer methods for the VTS
    /// </summary>
    public static class VtsJsonSerializer
    {
#if DEBUG
        /// <summary>
        /// memory trace writer
        /// </summary>
        public static MemoryTraceWriter TraceWriter = new MemoryTraceWriter();
#endif
        /// <summary>
        /// method to write to json
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myObject">object to write</param>
        /// <returns>written json</returns>
        public static string WriteToJson<T>(this T myObject)
        {
            var settings = new JsonSerializerSettings
            {
                //added temporarily to help serialize the sources that use interfaces
                TypeNameHandling = TypeNameHandling.None,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
#if DEBUG
            settings.TraceWriter = TraceWriter;
#endif
            settings.Converters.Add(new StringEnumConverter());
            string json = JsonConvert.SerializeObject(
                myObject,
                Formatting.Indented,
                settings);
#if DEBUG
            Console.WriteLine(TraceWriter);
#endif
            return json;
        }
        /// <summary>
        /// method to write json to file
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myObject">object to write</param>
        /// <param name="filename">json file written to</param>
        public static void WriteToJsonFile<T>(this T myObject, string filename)
        {
            var settingsJson = WriteToJson(myObject);

            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(settingsJson);
            }
        }
        /// <summary>
        /// list of known converters - one of each type of Monte Carlo class that is written to json
        /// </summary>
        public static List<JsonConverter> KnownConverters = new List<JsonConverter>
        {
            new ConventionBasedConverter<ISourceInput>(typeof(IsotropicPointSourceInput), "SourceType",  Vts.MonteCarlo.SourceType.BuiltInTypes),
            new ConventionBasedConverter<ITissueInput>(typeof(MultiLayerTissueInput), "TissueType", Vts.MonteCarlo.TissueType.BuiltInTypes),
            new ConventionBasedConverter<ITissueRegion>(typeof(VoxelTissueRegion), "TissueRegionType", TissueRegionType.BuiltInTypes),
            new ConventionBasedConverter<IDetectorInput>(typeof(ROfRhoDetectorInput), "TallyType", TallyType.BuiltInTypes),
            new ConventionBasedConverter<IDetector>(typeof(ROfRhoDetector), "TallyType", TallyType.BuiltInTypes),
            ConventionBasedConverter<ISourceProfile>.CreateFromEnum<Vts.MonteCarlo.SourceProfileType>(typeof(FlatSourceProfile)),
            new OpticalPropertiesConverter(),
        };
        /// <summary>
        /// method to read form json
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myString">string identifying json</param>
        /// <returns>deserialized object</returns>
        public static T ReadFromJson<T>(this string myString)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            foreach (var jsonConverter in KnownConverters)
            {
                serializer.Converters.Add(jsonConverter);
            }
            serializer.NullValueHandling = NullValueHandling.Ignore;
            //added temporarily to help serialize the sources that use interfaces
            serializer.TypeNameHandling = TypeNameHandling.None;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
#if DEBUG
            serializer.TraceWriter = TraceWriter;
#endif

            T deserializedProduct = default(T);
            using (var sr = new StringReader(myString))
            using (var reader = new JsonTextReader(sr))
            {
                deserializedProduct = serializer.Deserialize<T>(reader);
            }
#if DEBUG
            Console.WriteLine(TraceWriter);
#endif
            return deserializedProduct;
        }
        /// <summary>
        /// method to read json from file
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="filename">name of file to be read</param>
        /// <returns>json</returns>
        public static T ReadFromJsonFile<T>(string filename)
        {
            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Open))
            using (var sr = new StreamReader(stream, false))
            {
                var json = sr.ReadToEnd();

                return ReadFromJson<T>(json);
            }
        }
    }

    // from http://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net-to-deserialize-a-list-of-base
    /// <summary>
    /// class to deserialize json
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public class ConventionBasedConverter<TInterface> : JsonCreationConverter<TInterface> 
    {
        private static readonly UnityContainer _container;

        private readonly string _namespace;
        private readonly string _assemblyName;
        private readonly Type _interfaceType;
        private readonly string _classBasename;
        private readonly string _typeCategoryString;
        private readonly IDictionary<string, VtsClassInfo> _classInfoDictionary;
        
        /// <summary>
        /// Internal class for holding on to necessary class info for future instantiation
        /// </summary>
        class VtsClassInfo
        {
            public Type ClassType { get; set; }
            public string ClassName { get; set; }
            public string ClassPrefixString { get; set; }
        }

        /// <summary>
        /// Static constructor for VtsJsonSerializer
        /// </summary>
        static ConventionBasedConverter()
        {
            _container = new UnityContainer();
        }

        /// <summary>
        /// Returns an instance of ConventionBasedConverter
        /// </summary>
        /// <param name="exampleType">Type</param>
        /// <param name="typeCategoryString">string describing category</param>
        /// <param name="classPrefixStrings">IEnumerable of string</param>
        /// <param name="classBasename">name of base class</param>
        public ConventionBasedConverter(Type exampleType, string typeCategoryString, IEnumerable<string> classPrefixStrings,  string classBasename = null)
        {
            _namespace = exampleType.Namespace;
            _assemblyName = exampleType.Assembly.FullName;

            _interfaceType = typeof(TInterface);
            _classBasename = classBasename ?? _interfaceType.Name.Substring(1);
            _typeCategoryString = typeCategoryString;

            // the code: var useSingleton = false used to be here
            var useDefaultConstructor = true;
            
            var classList =
                from classPrefixString in classPrefixStrings
                let className = _namespace + @"." + classPrefixString + _classBasename
                let classType = Type.GetType(className + "," +  _assemblyName, false, true)
                select new VtsClassInfo
                {
                    ClassType = classType,
                    ClassName = className,
                    ClassPrefixString = classPrefixString,
                };

            foreach (var item in classList)
            {
                if (!object.Equals(item.ClassType, null))
                {
                    _container.RegisterType(
                        _interfaceType,
                        item.ClassType,
                        item.ClassPrefixString,
                        useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                }
            }

            _classInfoDictionary = classList.ToDictionary(item => item.ClassPrefixString);
        }
        /// <summary>
        /// method to create ConventionBasedConverter from enum
        /// </summary>
        /// <typeparam name="TEnum">enum</typeparam>
        /// <param name="exampleType">Type</param>
        /// <param name="classBasename">name of base class</param>
        /// <returns>ConventionBasedConverter</returns>
        public static ConventionBasedConverter<TInterface> CreateFromEnum<TEnum>(Type exampleType, string classBasename = null)
        {
            return new ConventionBasedConverter<TInterface>(
                exampleType,
                typeof(TEnum).Name,
                // use convention to map class names to enum types  e.g. ThingyType.First will register to FirstThingy 
                EnumHelper.GetValues<TEnum>().Select(value => value.ToString()),
                classBasename);
        }

        /// <summary>
        /// method to create TInterface given Type and JObject
        /// </summary>
        /// <param name="objectType">Type</param>
        /// <param name="jObject">JObject</param>
        /// <returns>TInterface</returns>
        protected override TInterface Create(Type objectType, JObject jObject)
        {
            if (!FieldExists(_typeCategoryString, jObject))
            {
                throw new Exception(String.Format("The given object type {0} is not supported!", objectType));
            }

            var classPrefixString = jObject[_typeCategoryString].ToString();
            
            //TEnum enumValue = default(TEnum);
            //string enumValueName = null;
            //if (Enum.TryParse(enumString, out enumValue)) // if it's an integer, representing an Enum class
            //{
            //    enumValueName = enumValue.ToString();
            //}
            //else
            //{
            //    enumValueName = enumString; // if it's a string, representing the actual name
            //}
            
            // get name of Enum from interface (e.g. if it's "IThingy", get "ThingyType" Enum and generate names for all source classes, and then use the corresponding factory, possibly also using convention "ThingyFactory")
            var classInfo = _classInfoDictionary[classPrefixString];

            var classInstance = _container.Resolve<TInterface>(classInfo.ClassPrefixString);

            return classInstance;
        }
        /// <summary>
        /// method to determine if field exists
        /// </summary>
        /// <param name="fieldName">name of field string</param>
        /// <param name="jObject">JObject object to check</param>
        /// <returns>boolean indicating if field exists</returns>
        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
    /// <summary>
    /// class JsonCreationConverter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);
        /// <summary>
        /// method to identify whether object can be converted
        /// </summary>
        /// <param name="objectType">object to be converted</param>
        /// <returns>boolean indicating whether object can be converted</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }
        /// <summary>
        /// method to read json
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="objectType">Type</param>
        /// <param name="existingValue">object: current type</param>
        /// <param name="serializer">JsonSerializer</param>
        /// <returns>object read</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
        /// <summary>
        /// method to write json
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">object to be written</param>
        /// <param name="serializer">JsonSerializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
