using ProxyFactoryCore.Abstract;
using ProxyFactoryCore.Impl;
using System;

namespace ProxyFactoryCore.Tests.Interceptors
{
    public class Interceptor1 : IInterceptor
    {
        public void AfterExecution(InvocationInfo invocationInfo)
        {
            invocationInfo.Result = new object();
            Console.WriteLine($"AFTER : {invocationInfo.Method.Name} ,{this.GetType().Name}");
        }

        public void BeforeExecution(InvocationInfo invocationInfo)
        {
            Console.WriteLine($"BEFORE : {invocationInfo.Method.Name} ,{this.GetType().Name}");
            invocationInfo.CancelMethodExecution();
        }
    }
}
