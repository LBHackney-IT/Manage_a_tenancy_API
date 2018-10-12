using System;

namespace Hackney.ServiceLocator
{
    public class ServiceAlreadyRegisteredException : Exception
    {
        private readonly string _serviceName; 
        public ServiceAlreadyRegisteredException(string serviceName)
        {
            _serviceName = serviceName;
        }
        public override string Message
        {
            get { return $"Service named '{_serviceName}' already added as Singleton Service."; }
        }
    }
}