using System;

namespace MediaDashboard.Persistence.Caching.Internal
{
    public abstract class BaseMdCache : IMdCache
    {
        public string SetAs<T>(string key, T value)
        {
            string serialized = ObjectSerializer.Serialize(value);
            Set(key, serialized);
            return serialized;
        }

        public string SetAs<T>(string key, T value, TimeSpan lifeSpan)
        {
            string serialized = ObjectSerializer.Serialize(value);
            Set(key, serialized, lifeSpan);
            return serialized;
        }

        public T GetAs<T>(string key) where T : class
        {
            string value = Get(key);
            if (null == value)
                return null;
            return ObjectSerializer.Deserialize(value) as T;
        }

        public abstract void Set(string key, string value);
        public abstract void Set(string key, string value, TimeSpan lifeSpan);
        public abstract string Get(string key);
    }
}
