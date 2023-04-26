using ProxyFactoryCore.Abstract.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Impl.Configuration
{
    public class MethodInterceptionConfiguration : IMethodInterceptionConfiguration
    {
        private readonly MethodInfo _methodInfo;

        public MethodInfo Method { get => _methodInfo; }

        public List<Type> Interceptors { get; set; } = new List<Type>();

        public Type ExceptionHandler { get; }

        internal MethodInterceptionConfiguration(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }
    }
}
