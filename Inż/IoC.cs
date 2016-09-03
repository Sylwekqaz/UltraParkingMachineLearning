using System;
using System.Collections.Generic;
using Ninject;

namespace Inż
{
    public static class IoC
    {
        private static IKernel _container;

        public static void Initialize(IKernel windsorContainer)
        {
            _container = windsorContainer;
        }

        public static T Resolve<T>()
        {
            return _container.Get<T>();
        }

        public static bool CanResolve<T>()
        {
            return _container.CanResolve<T>();
        }

        public static T Resolve<T>(string key)
        {
            return _container.Get<T>(key);
        }

        public static object Resolve(Type service)
        {
            return _container.Get(service);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return _container.GetAll<T>();
        }

        public static void Release(object instance)
        {
            _container.Release(instance);
        }
    }
}