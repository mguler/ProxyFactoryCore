using ProxyFactoryCore.Abstract;
using System;

namespace ProxyFactoryCore.Impl
{
    internal class DefaultExceptionHandler : IExceptionHandler
    {
        public void OnException(Exception ex)
        {

        }
        public void OnException(ArgumentException aex)
        {

        }
    }
}
