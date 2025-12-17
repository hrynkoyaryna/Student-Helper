using System;
using Microsoft.Extensions.DependencyInjection;

namespace StudentHelper.WPF.UI
{
    public static class ServiceLocator
    {
        private static IServiceProvider? _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : notnull
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceLocator is not initialized.");

            return _serviceProvider.GetRequiredService<T>();
        }

        public static object GetService(Type serviceType)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceLocator is not initialized.");

            return _serviceProvider.GetRequiredService(serviceType);
        }
    }
}
