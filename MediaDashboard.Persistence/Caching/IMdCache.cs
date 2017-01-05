using System;

namespace MediaDashboard.Persistence.Caching
{
    public interface IMdCache
    {
        string Get(string key);

        T GetAs<T>(string key) where T : class;

        void Set(string key, string value);

        void Set(string key, string value, TimeSpan lifeSpan);

        string SetAs<T>(string key, T value);

        string SetAs<T>(string key, T value, TimeSpan lifeSpan);
    }
}
