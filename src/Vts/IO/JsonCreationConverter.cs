using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vts.IO
{
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