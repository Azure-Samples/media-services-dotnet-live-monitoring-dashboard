using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaDashboard.Persistence.Caching.Internal
{
    internal static class ObjectSerializer
    {
        #region Helper class Envelope

        private class Envelope
        {
            public string TypeName { get; set; }
            public JObject Instance { get; set; }
        }

        #endregion

        public static string Serialize<T>(T value)
        {
            // set settings to handle derived types automatically and appropriately
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            JsonSerializer jserializer = JsonSerializer.Create(settings);

            JObject jobj = JObject.FromObject(value, jserializer);

            Envelope envelope = new Envelope();
            envelope.TypeName = value.GetType().AssemblyQualifiedName;
            envelope.Instance = jobj;

            return JsonConvert.SerializeObject(envelope);
        }

        public static object Deserialize(string serialized)
        {
            // set settings to handle derived types automatically and appropriately
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            JsonSerializer jserializer = JsonSerializer.Create(settings);

            Envelope envelope = JsonConvert.DeserializeObject<Envelope>(serialized);
            object obj = null;
            if (envelope.Instance != null)
            {
                Type type = Type.GetType(envelope.TypeName);
                obj = envelope.Instance.ToObject(type, jserializer);
            }
            

            return ((obj !=null)? obj : serialized);
        }
    }
}
