using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace component.helper
{
    public class DependencyResolver
    {
        private static DependencyResolver _resolver;
        private readonly IServiceProvider _serviceProvider;

        private DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static DependencyResolver Current
        {
            get
            {
                if (_resolver == null)
                    throw new Exception("AppDependencyResolver Not initialized. You should initialize in startup class");
                return _resolver;
            }
        }

        public static void Init(IServiceCollection services)
        {
            _resolver = new DependencyResolver(services.BuildServiceProvider());
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public T GetService<T>()
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }
    }
}
