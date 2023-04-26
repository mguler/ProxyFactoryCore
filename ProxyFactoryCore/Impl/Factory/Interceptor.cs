using ProxyFactoryCore.Abstract.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProxyFactoryCore.Impl.Factory
{
    public class Interceptor
    {
        public object Intercept(object instance, object[] args, Type[] interceptorTypes, nint methodHandle)
        {
            var v = instance.GetType().GetMembers();

            var methodInfo = MethodBase.GetMethodFromHandle(RuntimeMethodHandle.FromIntPtr(methodHandle)) as MethodInfo;
            var parameters = methodInfo.GetParameters();
            var parameterTypes = parameters.Select(parameterInfo => parameterInfo.ParameterType).ToArray();
            var methodInvokerInfo = instance.GetType().GetMethod($"@________{methodInfo.Name}", parameterTypes);
            var result = default(object);

            var invocationInfo = new InvocationInfo(instance, methodInfo) as IInvocationInfo;
            for (var index = 0; index < parameters.Length; index++)
            {
                invocationInfo.Parameters.Add(parameters[index].Name, args[index]);
            }

            try
            {
                var interceptors = new List<IInterceptor>();
                for (var index = 0; index < interceptorTypes.Length; index++)
                {
                    var interceptorConstructorInfo = interceptorTypes[index].GetConstructor(Type.EmptyTypes);
                    var interceptorInstance = interceptorConstructorInfo.Invoke(null) as IInterceptor;
                    interceptors.Add(interceptorInstance);

                    invocationInfo = interceptorInstance.BeforeExecution(invocationInfo);
                }

                if (invocationInfo.IsCancelled)
                {
                    return invocationInfo.Result;
                }

                invocationInfo.Result = methodInvokerInfo.Invoke(instance, args);

                for (var index = 0; index < interceptors.Count; index++)
                {
                    invocationInfo = interceptors[index].AfterExecution(invocationInfo);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return invocationInfo.Result;
        }
    }
}
