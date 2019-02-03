using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vts
{
    /// <summary>
    /// Describes optical properties Converter for deserializing OPs based on (mua, mus, g, n)
    /// not on (mua, musp, g, n)
    /// ref: https://stackoverflow.com/questions/23017716/json-net-how-to-deserialize-without-using-the-default-constructor/23017892#23017892
    [DataContract]
    public class OpticalPropertiesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(OpticalProperties));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            // Load the JSON for the OpticalProperties into a JObject
            JObject jo = JObject.Load(reader);

            // Read the properties which will be used as constructor parameters
            double mua = (double) jo["Mua"];
            double g = (double)jo["G"];
            double n = (double)jo["N"];
            double mus = 0, musp = 0; // possible trouble setting these to 0
            // check Musp last so that it forces Mus as the code used to
            if (jo["Mus"] != null) // Mus in OPs
            {
                mus = (double)jo["Mus"];
                if (g == 1)
                {
                    musp = mus;
                }
                else
                {
                    musp = mus * (1 - g);
                }
            }
            if (jo["Musp"] != null) // Musp in OPs
            {
                musp = (double)jo["Musp"];
            }
           
            // Construct the OpticalProperties using the non-default constructor
            OpticalProperties opticalProperties = new OpticalProperties(mua, musp, g, n);

            // (If anything else needs to be populated on the result object, do that here)

            return opticalProperties;
        }


        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}