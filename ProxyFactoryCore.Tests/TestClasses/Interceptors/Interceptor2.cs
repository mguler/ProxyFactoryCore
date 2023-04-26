using ProxyFactoryCore.Abstract.Factory;

namespace ProxyFactoryCore.Tests.TestClasses.Interceptors
{
    public class Interceptor2 : IInterceptor
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
