using ProxyFactoryCore.Abstract;
using ProxyFactoryCore.Impl;
using System;
using System.Diagnostics;
using System.Linq;

namespace ProxyFactoryCore.Tests.Interceptors
{
    public class Interceptor2 : IInterceptor
    {
        public void AfterExecution(InvocationInfo invocationInfo)
        {
            Console.WriteLine($"AFTER : {invocationInfo.Method.Name} ,{this.GetType().Name}");
        }

        public void BeforeExecution(InvocationInfo invocationInfo)
        {
            Console.WriteLine($"BEFORE : {invocationInfo.Method.Name} ,{this.GetType().Name}");
        }
    }
}
