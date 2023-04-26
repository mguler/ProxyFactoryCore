using ProxyFactoryCore.Abstract.Factory;
using System;

namespace ProxyFactoryCore.Tests.TestClasses.Interceptors
{
    public class Interceptor1 : IInterceptor
    {
        public IInvocationInfo AfterExecution(IInvocationInfo invocationInfo)
        {
            return invocationInfo;
        }

        public IInvocationInfo BeforeExecution(IInvocationInfo invocationInfo)
        {
            return invocationInfo;
        }
    }
}
