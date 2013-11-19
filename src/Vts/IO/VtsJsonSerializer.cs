using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Meta.Numerics.Statistics;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.IO
{
    public static class VtsJsonSerializer
    {
#if DEBUG
        private static MemoryTraceWriter _traceWriter = new MemoryTraceWriter();
#endif
        public static string WriteToJson<T>(this T myObject)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None, 
            };
#if DEBUG
            settings.TraceWriter = _traceWriter;
#endif
            settings.Converters.Add(new StringEnumConverter());
            string json = JsonConvert.SerializeObject(
                myObject,
                Formatting.Indented,
                settings);
#if DEBUG
            Console.WriteLine(_traceWriter);
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
            new ConventionBasedConverter<Vts.MonteCarlo.SourceType, ISourceInput>(typeof(IsotropicPointSourceInput)),
            new ConventionBasedConverter<Vts.MonteCarlo.TissueType, ITissueInput>(typeof(MultiLayerTissueInput)),
            new ConventionBasedConverter<Vts.MonteCarlo.TissueRegionType, ITissueRegion>(typeof(VoxelRegion), "Region"),
            new ConventionBasedConverter<Vts.MonteCarlo.TallyType, IDetectorInput>(typeof(ROfRhoDetectorInput)),
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
#if DEBUG
            serializer.TraceWriter = _traceWriter;
#endif

            T deserializedProduct = default(T);
            using (var sr = new StringReader(myString))
            using (var reader = new JsonTextReader(sr))
            {
                deserializedProduct = serializer.Deserialize<T>(reader);
            }
#if DEBUG
            Console.WriteLine(_traceWriter);
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
    public class ConventionBasedConverter<TEnum, TInterface> : JsonCreationConverter<TInterface> where TEnum : struct
    {
        private static readonly UnityContainer _container;

        private readonly string _namespace;
        private readonly string _assemblyName;
        private readonly Type _interfaceType;
        private readonly string _classBasename;
        private readonly Type _enumType;
        private readonly string _enumName;
        private readonly IDictionary<string, VtsClassInfo> _classInfoDictionary;
        
        /// <summary>
        /// Internal class for holding on to necessary class info for future instantiation
        /// </summary>
        class VtsClassInfo
        {
            public Type ClassType { get; set; }
            public string ClassName { get; set; }
            public string EnumValueName { get; set; }
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
        public ConventionBasedConverter(Type exampleType, string classBasename = null)
        {
            _namespace = exampleType.Namespace;
            _assemblyName = exampleType.Assembly.FullName;

            _interfaceType = typeof(TInterface);
            _classBasename = classBasename ?? _interfaceType.Name.Substring(1);
            _enumType = typeof(TEnum);
            _enumName = _enumType.Name;

            // use convention to map class names to enum types  e.g. ThingyType.First will register to FirstThingy 
            var enumValues = EnumHelper.GetValues<TEnum>();
            var useSingleton = true;
            var useDefaultConstructor = true;
            
            var classList = 
                from enumValue in enumValues
                let className = _namespace + @"." + enumValue + _classBasename
                let classType = Type.GetType(className + "," +  _assemblyName, false, true)
                let enumValueName = enumValue.ToString()
                select new VtsClassInfo
                {
                    ClassType = classType,
                    ClassName = className,
                    EnumValueName = enumValueName,
                };

            foreach (var item in classList)
            {
                if (!object.Equals(item.ClassType, null))
                {
                    _container.RegisterType(
                        _interfaceType,
                        item.ClassType,
                        item.EnumValueName, // use the enum string to register each class
                        useSingleton ? new ContainerControlledLifetimeManager() : null,
                        useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                }
            }

            _classInfoDictionary = classList.ToDictionary(item => item.EnumValueName);
        }
        
        protected override TInterface Create(Type objectType, JObject jObject)
        {
            if (!FieldExists(_enumName, jObject))
            {
                throw new Exception(String.Format("The given object type {0} is not supported!", objectType));
            }

            var enumString = jObject[_enumName].ToString();
            
            TEnum enumValue = default(TEnum);
            string enumValueName = null;
            if (Enum.TryParse(enumString, out enumValue)) // if it's an integer, representing an Enum class
            {
                enumValueName = enumValue.ToString();
            }
            else
            {
                enumValueName = enumString; // if it's a string, representing the actual name
            }
            
            // get name of Enum from interface (e.g. if it's "IThingy", get "ThingyType" Enum and generate names for all source classes, and then use the corresponding factory, possibly also using convention "ThingyFactory")
            var classInfo = _classInfoDictionary[enumValueName];

            var classInstance = _container.Resolve<TInterface>(classInfo.EnumValueName);

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
