using ProxyFactoryCore.Abstract.Factory;

namespace ProxyFactoryCore.Abstract.Configuration
{
    public interface IMethodInterceptionConfigurationBuilder
    {
        IMethodInterceptionConfigurationBuilder AddInterceptor<TInterceptor>() where TInterceptor : IInterceptor;
        IMethodInterceptionConfigurationBuilder SetExceptionHandler<TExceptionHandler>() where TExceptionHandler : IExceptionHandler;
    }
}
