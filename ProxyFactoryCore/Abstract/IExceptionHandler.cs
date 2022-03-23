using ProxyFactory.Core.Abstract;
using System;

namespace ProxyFactoryCore.Abstract
{
    public interface IExceptionHandler
    {
        void OnException(Exception ex, IInvocationInfo invocationInfo);
    }
}
