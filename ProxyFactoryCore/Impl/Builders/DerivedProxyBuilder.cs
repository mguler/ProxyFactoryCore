using System.Reflection.Emit;
using System.Reflection;
using ProxyFactoryCore.Abstract.Configuration;
using ProxyFactoryCore.Impl.Factory;
using ProxyFactoryCore.Abstract.Builders;
using System;
using System.Linq;

namespace ProxyFactoryCore.Impl.Builders
{
    public class DerivedProxyBuilder : IProxyBuilder
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private TypeBuilder _typeBuilder;
        private Type _baseType;

        public DerivedProxyBuilder()
        {
            var assemblyName = new AssemblyName("DynamicAssembly");
            var moduleName = "DynamicModule";
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(moduleName);
        }

        public Type Build(IProxyTypeConfiguration proxyTypeConfiguration)
        {
            _baseType = proxyTypeConfiguration.Type;
            _typeBuilder = _moduleBuilder.DefineType($"{_baseType.Name}Proxy", TypeAttributes.Public, _baseType, null);

            if (_baseType.IsGenericType)
            {
                var generics = _baseType.GetGenericArguments();
                _typeBuilder.DefineGenericParameters(generics.Select(genericBuilder => genericBuilder.Name).ToArray());
            }

            OverrideConstructors();

            foreach (var methodConfig in proxyTypeConfiguration.MethodInterceptionConfiguration)
            {
                BuildMethod(methodConfig.Value);
            }

            var proxyType = _typeBuilder.CreateType();
            return proxyType;
        }

        private MethodBuilder BuildMethod(IMethodInterceptionConfiguration methodConfig)
        {
            var prototype = methodConfig.Method;
            var interceptorType = typeof(Interceptor);
            var interceptor = interceptorType.GetConstructor(Type.EmptyTypes);
            var intercept = interceptorType.GetMethod("Intercept");
            var typeFromRuntimeHandle = typeof(Type).GetMethod("GetTypeFromHandle");

            var derivedMethodBuilder = _typeBuilder.DefineMethod(prototype.Name
                , MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final
                , prototype.CallingConvention
                , prototype.ReturnType
                , prototype.ReturnParameter?.GetRequiredCustomModifiers()
                , prototype.ReturnParameter?.GetOptionalCustomModifiers()
                , prototype.GetParameters().Select(parameter => parameter.ParameterType).ToArray()
                , prototype.GetParameters().Select(parameter => parameter.GetRequiredCustomModifiers()).ToArray()
                , prototype.GetParameters().Select(parameter => parameter.GetOptionalCustomModifiers()).ToArray());

            var invokeBaseBuilder = _typeBuilder.DefineMethod($"@________{prototype.Name}"
            , MethodAttributes.Public | MethodAttributes.Final
            , prototype.CallingConvention
            , prototype.ReturnType
            , prototype.ReturnParameter?.GetRequiredCustomModifiers()
            , prototype.ReturnParameter?.GetOptionalCustomModifiers()
            , prototype.GetParameters().Select(parameter => parameter.ParameterType).ToArray()
            , prototype.GetParameters().Select(parameter => parameter.GetRequiredCustomModifiers()).ToArray()
            , prototype.GetParameters().Select(parameter => parameter.GetOptionalCustomModifiers()).ToArray());

            var methodParameters = prototype.GetParameters().ToArray();

            #region Create Base Invoker
            var baseInvokerIlGenerator = invokeBaseBuilder.GetILGenerator();
            baseInvokerIlGenerator.Emit(OpCodes.Ldarg_0);
            for (var index = 0; index < methodParameters.Length; index++)
            {
                baseInvokerIlGenerator.Emit(OpCodes.Ldarg, index + 1);
            }
            baseInvokerIlGenerator.Emit(OpCodes.Call, prototype);
            baseInvokerIlGenerator.Emit(OpCodes.Ret);
            #endregion End Of Create Base Invoker 

            #region Create Proxy Method
            var ilGenerator = derivedMethodBuilder.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(object[]));
            ilGenerator.DeclareLocal(typeof(object[]));

            //Create an array of objects
            ilGenerator.Emit(OpCodes.Ldc_I4, methodParameters.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_0);

            //And add the parameters passed to the method into the array.
            if (methodParameters.Any())
            {
                for (var index = 0; index < methodParameters.Length; index++)
                {
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldc_I4, index);
                    ilGenerator.Emit(OpCodes.Ldarg, index + 1);
                    ilGenerator.Emit(OpCodes.Box, methodParameters[index].ParameterType);
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }

            //Create an array of object
            ilGenerator.Emit(OpCodes.Ldc_I4, methodConfig.Interceptors.Count);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_1);

            //And add the interceptor types
            for (var index = 0; index < methodConfig.Interceptors.Count; index++)
            {
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ldc_I4, index);
                ilGenerator.Emit(OpCodes.Ldtoken, methodConfig.Interceptors[index]);
                ilGenerator.Emit(OpCodes.Call, typeFromRuntimeHandle);
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }

            ////Invoke the default interceptor
            ilGenerator.Emit(OpCodes.Newobj, interceptor);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldc_I8, prototype.MethodHandle.Value);
            ilGenerator.Emit(OpCodes.Call, intercept);

            //if (prototype.ReturnType.IsPrimitive)
            //{
            //    ilGenerator.Emit(OpCodes.Unbox_Any, prototype.ReturnType);
            //}

            ilGenerator.Emit(OpCodes.Ret);
            #endregion End Of Create Proxy Method

            return derivedMethodBuilder;
        }
        private void OverrideConstructors()
        {
            var constructors = _baseType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            foreach (var constructorInfo in constructors)
            {

                var constructorBuilder = _typeBuilder.DefineConstructor(constructorInfo.Attributes
                 , constructorInfo.CallingConvention
                 , constructorInfo.GetParameters().Select(parameter => parameter.ParameterType).ToArray()
                 , constructorInfo.GetParameters().Select(parameter => parameter.GetRequiredCustomModifiers()).ToArray()
                 , constructorInfo.GetParameters().Select(parameter => parameter.GetOptionalCustomModifiers()).ToArray());

                var constructorParameters = constructorInfo.GetParameters().ToArray();
                var ilGenerator = constructorBuilder.GetILGenerator();

                for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
                {
                    var methodParameter = constructorParameters[parameterIndex];
                    var derivedMethodParameterBuilder = constructorBuilder.DefineParameter(parameterIndex + 1,
                        ParameterAttributes.None,
                        methodParameter.Name);
                }

                #region Prepare The Parameters
                //Load current instance "this"
                ilGenerator.Emit(OpCodes.Ldarg_0);

                for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
                {
                    //Load the argument at the index
                    ilGenerator.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
                #endregion End Of Prepare The Parameters

                ilGenerator.Emit(OpCodes.Call, constructorInfo);
                ilGenerator.Emit(OpCodes.Ret);
            }
        }
    }
}
