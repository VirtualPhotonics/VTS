using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vts.IO
{
    /// <summary>
    /// class JsonCreationConverter
    /// </summary>
    /// <typeparam name="T">generic type T</typeparam>
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">The type of object expected</param>
        /// <param name="jObject">The contents of JSON object that will be deserialized</param>
        /// <returns>A generic type T</returns>
        protected abstract T Create(Type objectType, JObject jObject);
        /// <summary>
        /// Method to identify whether object can be converted
        /// </summary>
        /// <param name="objectType">The object to be converted</param>
        /// <returns>A Boolean indicating whether object can be converted</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Method to populate JSON values into the target object
        /// </summary>
        /// <param name="reader">The JsonReader with the values</param>
        /// <param name="objectType">The target object type</param>
        /// <param name="existingValue">This value is not used</param>
        /// <param name="serializer">The JsonSerializer</param>
        /// <returns>An object of the target type</returns>
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
        /// method to write JSON - Not implemented
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