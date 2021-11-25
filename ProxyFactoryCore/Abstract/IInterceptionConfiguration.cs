using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ProxyFactoryCore.Abstract
{
    public interface IInterceptorConfiguration
    {
        Type[] GetInterceptors(MethodInfo methodInfo);
    }
    public interface IInterceptorConfiguration<T> : IInterceptorConfiguration
    {
        Type ProxyType { get; set; }
        Type ProxyBuilderType { get; set; }

        IInterceptorConfiguration<T2> Add<T2>() where T2 : class;
        IInterceptorConfiguration<T> With<TInterceptor>() where TInterceptor : class, IInterceptor;
        IInterceptorConfiguration<T> Intercept<TResult>(Expression<Func<T, TResult>> expression);
        IInterceptorConfiguration<T> Intercept<TResult>(Expression<Action<T>> expression);
        IInterceptorConfiguration<T> UsingBuilder<TProxyBuilder>();

    }
}
