using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Unity;
using Unity.Injection;

namespace Vts.IO
{
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
                throw new ArgumentException(String.Format("The given object type {0} is not supported!", objectType));
            }

            var classPrefixString = jObject[_typeCategoryString].ToString();

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
}