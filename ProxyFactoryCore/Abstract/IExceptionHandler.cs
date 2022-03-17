using System;

namespace ProxyFactoryCore.Abstract
{
    public interface IExceptionHandler
    {
        void OnException(Exception ex);
    }
}
