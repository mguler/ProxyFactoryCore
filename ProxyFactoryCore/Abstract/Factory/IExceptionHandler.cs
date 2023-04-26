using System;

namespace ProxyFactoryCore.Abstract.Factory
{
    public interface IExceptionHandler
    {
        void OnException(Exception exception, IInvocationInfo invocationInfo);
    }
}
