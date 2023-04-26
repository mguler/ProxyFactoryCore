using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Abstract.Configuration
{
    public interface IMethodInterceptionConfiguration
    {
        MethodInfo Method { get; }
        Type ExceptionHandler { get; }
        List<Type> Interceptors { get; }
    }
}
