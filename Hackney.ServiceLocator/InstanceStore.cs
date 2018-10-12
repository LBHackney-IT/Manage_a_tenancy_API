using System.Collections.Generic;
using Hackney.InterfaceStubs;

namespace Hackney.ServiceLocator
{
    public static class SingletonInstanceStore
    {
        private static readonly Dictionary<string, object> SingletonObjects = new Dictionary<string, object>();
        private static readonly object dictionaryLock = new object();
        public static T GetSingletonObject<T>()
            where T : class, IService
        {
            string key = typeof(T).ToString();
            T singletonObject;
            lock (dictionaryLock)
            {
                if (!SingletonObjects.ContainsKey(key))
                    return null;

                singletonObject = SingletonObjects[key] as T;
            }

            return singletonObject;
        }

        /// <summary>
        /// Register a singleton object reference of the service in the store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="singletonObject"></param>
        public static void RegisterSingletonObject<T>(T singletonObject)
            where T : class, IService
        {
            lock (dictionaryLock)
            {
                if (SingletonObjects.ContainsKey(typeof(T).ToString()))
                {
                    throw new ServiceAlreadyRegisteredException(typeof(T).ToString());
                }

                SingletonObjects.Add(typeof(T).ToString(), singletonObject);
            }
        }
    }
}
