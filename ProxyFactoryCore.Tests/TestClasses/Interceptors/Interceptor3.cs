using ProxyFactoryCore.Abstract;
using ProxyFactoryCore.Impl;
using System;

namespace ProxyFactoryCore.Tests.Interceptors
{
    public class Interceptor3 : IInterceptor
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
