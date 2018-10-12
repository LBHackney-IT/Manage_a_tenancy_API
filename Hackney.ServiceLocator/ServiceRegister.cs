using System;
using System.IO;
using System.Reflection;
using Hackney.InterfaceStubs;

namespace Hackney.ServiceLocator
{
    
    public static class ServiceRegister<T>
        where T : class, IService
    {
        private static readonly object _lockInstantiation = new object();
        
        public static T InstantiateService(string serviceName, string serviceLocationPath, InstancingType instancingType)
        {
            if (serviceLocationPath == null) throw new ArgumentNullException(nameof(serviceLocationPath));
            lock (_lockInstantiation)
            {
                T serviceInstance = SingletonInstanceStore.GetSingletonObject<T>();
                if (serviceInstance != null)
                    return serviceInstance;


                foreach (var file in Directory.GetFiles(serviceLocationPath, "*.dll"))
                {
                    Assembly asm = Assembly.LoadFile(Path.Combine(serviceLocationPath, file));
                    if (!file.EndsWith("Hackney.InterfaceStubs.dll"))
                        serviceInstance = ServiceActivator(asm, serviceName);
                    if (serviceInstance != null)
                        break;
                }

                if (instancingType == InstancingType.Singleton)
                {
                    SingletonInstanceStore.RegisterSingletonObject(serviceInstance);
                }
                return serviceInstance;
            }
        }

        private static T ServiceActivator(Assembly asm, string serviceName)
        {
            foreach (var asmType in asm.ExportedTypes)
            {
                if (typeof(T).IsAssignableFrom(asmType))
                {
                    
                    T serviceInstance = (T)Activator.CreateInstance(asmType);
                    if (serviceInstance.ServiceName == serviceName)
                    {
                        return serviceInstance;
                    }
                }
            }

            return null;
        }
    }
}
