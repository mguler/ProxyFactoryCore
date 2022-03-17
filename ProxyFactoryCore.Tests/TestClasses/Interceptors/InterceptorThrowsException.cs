using ProxyFactoryCore.Abstract;
using ProxyFactoryCore.Impl;
using System;

namespace ProxyFactoryCore.Tests.TestClasses.Interceptors
{
    public class InterceptorThrowsException : IInterceptor
    {
        public void AfterExecution(InvocationInfo invocationInfo)
        {
            throw new Exception();
        }

        public void BeforeExecution(InvocationInfo invocationInfo)
        {

        }
    }
}
