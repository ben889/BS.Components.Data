namespace BS.Components.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ObjectCache
    {
        private static Dictionary<string, List<string>> columnsCache = null;
        private static object columnsCacheLocker = new object();
        private static Dictionary<string, PropertyInfo> propertyInfoCache = null;
        private static object propertyInfoCacheLocker = new object();
        private static Dictionary<string, string> stringCache = null;
        private static object stringCacheLocker = new object();

        static ObjectCache()
        {
            stringCache = new Dictionary<string, string>();
            propertyInfoCache = new Dictionary<string, PropertyInfo>();
            columnsCache = new Dictionary<string, List<string>>();
        }

        public static List<string> GetColumns(string key)
        {
            if (columnsCache.ContainsKey(key))
            {
                return columnsCache[key];
            }
            return null;
        }

        public static PropertyInfo GetPropertyInfo(string key)
        {
            if (propertyInfoCache.ContainsKey(key))
            {
                return propertyInfoCache[key];
            }
            return null;
        }

        public static string GetString(string key)
        {
            if (stringCache.ContainsKey(key))
            {
                return stringCache[key];
            }
            return null;
        }

        public static void SetColumns(string key, List<string> value)
        {
            object obj2;
            object obj3;
            if (!columnsCache.ContainsKey(key))
            {
                lock ((obj3 = obj2 = columnsCacheLocker))
                {
                    if (!columnsCache.ContainsKey(key))
                    {
                        columnsCache.Add(key, null);
                    }
                }
            }
            lock ((obj3 = obj2 = columnsCacheLocker))
            {
                columnsCache[key] = value;
            }
        }

        public static void SetPropertyInfo(string key, PropertyInfo value)
        {
            object obj2;
            object obj3;
            if (!propertyInfoCache.ContainsKey(key))
            {
                lock ((obj3 = obj2 = propertyInfoCacheLocker))
                {
                    if (!propertyInfoCache.ContainsKey(key))
                    {
                        propertyInfoCache.Add(key, null);
                    }
                }
            }
            lock ((obj3 = obj2 = propertyInfoCacheLocker))
            {
                propertyInfoCache[key] = value;
            }
        }

        public static void SetString(string key, string value)
        {
            object obj2;
            object obj3;
            if (!stringCache.ContainsKey(key))
            {
                lock ((obj3 = obj2 = stringCacheLocker))
                {
                    if (!stringCache.ContainsKey(key))
                    {
                        stringCache.Add(key, null);
                    }
                }
            }
            lock ((obj3 = obj2 = stringCacheLocker))
            {
                stringCache[key] = value;
            }
        }
    }
}

