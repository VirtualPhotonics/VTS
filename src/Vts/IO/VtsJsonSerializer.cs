using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Practices.Unity;
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
    public static class VtsJsonSerializer
    {
#if DEBUG
        public static MemoryTraceWriter TraceWriter = new MemoryTraceWriter();
#endif
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

        public static void WriteToJsonFile<T>(this T myObject, string filename)
        {
            var settingsJson = WriteToJson(myObject);

            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(settingsJson);
            }
        }

        public static List<JsonConverter> KnownConverters = new List<JsonConverter>
        {
            new ConventionBasedConverter<ISourceInput>(typeof(IsotropicPointSourceInput), "SourceType",  Vts.MonteCarlo.SourceType.BuiltInTypes),
            new ConventionBasedConverter<ITissueInput>(typeof(MultiLayerTissueInput), "TissueType", Vts.MonteCarlo.TissueType.BuiltInTypes),
            new ConventionBasedConverter<ITissueRegion>(typeof(VoxelTissueRegion), "TissueRegionType", TissueRegionType.BuiltInTypes),
            new ConventionBasedConverter<IDetectorInput>(typeof(ROfRhoDetectorInput), "TallyType", TallyType.BuiltInTypes),
            new ConventionBasedConverter<IDetector>(typeof(ROfRhoDetector), "TallyType", TallyType.BuiltInTypes),
            ConventionBasedConverter<ISourceProfile>.CreateFromEnum<Vts.MonteCarlo.SourceProfileType>(typeof(FlatSourceProfile)),
            
        };

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
        /// <param name="exampleType"></param>
        /// <param name="classBasename"></param>
        public ConventionBasedConverter(Type exampleType, string typeCategoryString, IEnumerable<string> classPrefixStrings,  string classBasename = null)
        {
            _namespace = exampleType.Namespace;
            _assemblyName = exampleType.Assembly.FullName;

            _interfaceType = typeof(TInterface);
            _classBasename = classBasename ?? _interfaceType.Name.Substring(1);
            _typeCategoryString = typeCategoryString;

            var useSingleton = false;
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
                        item.ClassPrefixString, // use the prefix string to register each class
                        useSingleton ? new ContainerControlledLifetimeManager() : null,
                        useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                }
            }

            _classInfoDictionary = classList.ToDictionary(item => item.ClassPrefixString);
        }
        
        public static ConventionBasedConverter<TInterface> CreateFromEnum<TEnum>(Type exampleType, string classBasename = null)
        {
            return new ConventionBasedConverter<TInterface>(
                exampleType,
                typeof(TEnum).Name,
                // use convention to map class names to enum types  e.g. ThingyType.First will register to FirstThingy 
                EnumHelper.GetValues<TEnum>().Select(value => value.ToString()),
                classBasename);
        }

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

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
