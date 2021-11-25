using ProxyFactoryCore.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ProxyFactoryCore.Impl
{
    public class InterceptionConfiguration<T> : IInterceptorConfiguration<T>
    {
        private readonly IDynamicProxyFactory _dynamicProxyFactory;
        private readonly Dictionary<MethodInfo, List<Type>> _cache = new Dictionary<MethodInfo, List<Type>>();
        private List<Type> _currentInterceptorCache;

        public Type Type { get => typeof(T); }
        public Type ProxyType { get; set; }
        public Type ProxyBuilderType { get; set; } = typeof(DerivedProxyBuilder);

        public InterceptionConfiguration(IDynamicProxyFactory dynamicProxyFactory)
        {
            _dynamicProxyFactory = dynamicProxyFactory;
            _dynamicProxyFactory.Add(this);
        }
        public IInterceptorConfiguration<T2> Add<T2>() where T2 : class
        {
            return new InterceptionConfiguration<T2>(_dynamicProxyFactory);
        }

        public IInterceptorConfiguration<T> Intercept<TResult>(Expression<Func<T, TResult>> expression)
        {
            var methodInfo = (expression.Body as MethodCallExpression).Method;
            SetCurrentInterceptorCache(methodInfo);
            return this;
        }

        public IInterceptorConfiguration<T> Intercept<TResult>(Expression<Action<T>> expression)
        {
            var methodInfo = (expression.Body as MethodCallExpression).Method;
            SetCurrentInterceptorCache(methodInfo);
            return this;
        }

        public IInterceptorConfiguration<T> With<TInterceptor>() where TInterceptor : class, IInterceptor
        {
            _currentInterceptorCache.Add(typeof(TInterceptor));
            return this;
        }
        public IInterceptorConfiguration<T> UsingBuilder<TProxyBuilder>()
        {
            return null;
        }
        public Type[] GetInterceptors(MethodInfo methodInfo)
        {
            var interceptorTypes = _cache.Keys.Contains(methodInfo) ? _cache[methodInfo].ToArray() : null;
            return interceptorTypes;
        }
        private void SetCurrentInterceptorCache(MethodInfo methodInfo)
        {
            _currentInterceptorCache = _cache.FirstOrDefault(interceptorCache => interceptorCache.Key == methodInfo).Value;
            if (_currentInterceptorCache == null)
            {
                _cache.Add(methodInfo, new List<Type>());
                _currentInterceptorCache = _cache[methodInfo];
            }
        }
    }
}
