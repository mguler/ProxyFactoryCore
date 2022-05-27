using ProxyFactory.Core.Abstract;
using ProxyFactoryCore.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProxyFactoryCore.Impl
{
    public class DerivedProxyBuilder : IProxyBuilder
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private TypeBuilder _typeBuilder;
        private Type _baseType;
        public DerivedProxyBuilder()
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Assembly.GetExecutingAssembly().FullName), AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
        }

        public Type CreateProxyType<T1>(IInterceptorConfiguration config)
        {
            return CreateProxyType(typeof(T1), config);
        }
        public Type CreateProxyType(Type baseType, IInterceptorConfiguration config)
        {
            _baseType = baseType;
            _typeBuilder = _moduleBuilder.DefineType($"{_baseType.FullName}Proxy", _baseType.Attributes, _baseType);
            var before = typeof(IInterceptor).GetMethod("BeforeExecution", new Type[] { typeof(InvocationInfo) });
            var after = typeof(IInterceptor).GetMethod("AfterExecution", new Type[] { typeof(InvocationInfo) });
            var parameters = typeof(Dictionary<string, object>).GetConstructor(new Type[0]);
            var addParameter = typeof(Dictionary<string, object>).GetMethod("Add", new Type[] { typeof(string), typeof(object) });
            var getMethod = typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string) ,typeof(Type[]) });
            var typeFromRuntimeHandle = typeof(Type).GetMethod("GetTypeFromHandle");
            var methods = _baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var interceptorCache = typeof(List<IInterceptor>).GetConstructor(Type.EmptyTypes);
            var addInterceptor = typeof(List<IInterceptor>).GetMethod("Add", new Type[] { typeof(IInterceptor) });
            var invocationInfo = typeof(InvocationInfo).GetConstructor(new Type[] { typeof(object), typeof(object), typeof(MethodInfo), typeof(Dictionary<string, object>) });
            var setResult = typeof(InvocationInfo).GetProperty("Result").GetSetMethod();
            var getResult = typeof(InvocationInfo).GetProperty("Result").GetGetMethod();
            var isCancelled = typeof(InvocationInfo).GetField("_cancelExecution", BindingFlags.Public | BindingFlags.Instance);
            var bypass = typeof(InvocationInfo).GetField("_bypassInterceptors", BindingFlags.Public | BindingFlags.Instance);

            var getItem = typeof(List<IInterceptor>).GetProperty("Item").GetGetMethod();
            var getCount = typeof(List<IInterceptor>).GetProperty("Count").GetGetMethod();
            var invoke = typeof(MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) });
            var getType = typeof(object).GetMethod("GetType", Type.EmptyTypes);

            foreach (var methodInfo in methods)
            {
                if (!methodInfo.IsVirtual)
                {
                    continue;
                }

                // Get Interceptor Cache For Current Method 
                var interceptors = config.GetInterceptors(methodInfo);

                //If There Is No Interceptors Defined For Current Method
                if (interceptors == null || interceptors.Length == 0)
                {
                    //Continue to Next Method
                    continue;
                }

                var derivedMethodBuilder = _typeBuilder.DefineMethod(methodInfo.Name
                    , MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final
                    , methodInfo.CallingConvention
                    , methodInfo.ReturnType
                    , methodInfo.ReturnParameter?.GetRequiredCustomModifiers()
                    , methodInfo.ReturnParameter?.GetOptionalCustomModifiers()
                    , methodInfo.GetParameters().Select(parameter => parameter.ParameterType).ToArray()
                    , methodInfo.GetParameters().Select(parameter => parameter.GetRequiredCustomModifiers()).ToArray()
                    , methodInfo.GetParameters().Select(parameter => parameter.GetOptionalCustomModifiers()).ToArray());

                var methodParameters = methodInfo.GetParameters().ToArray();
                var ilGenerator = derivedMethodBuilder.GetILGenerator();

                derivedMethodBuilder.SetReturnType(methodInfo.ReturnType);

                for (var parameterIndex = 0; parameterIndex < methodParameters.Length; parameterIndex++)
                {
                    var methodParameter = methodParameters[parameterIndex];
                    var derivedMethodParameterBuilder = derivedMethodBuilder.DefineParameter(parameterIndex + 1,
                        ParameterAttributes.None,
                        methodParameter.Name);
                }

                //Declare local variables 
                ilGenerator.DeclareLocal(typeof(List<IInterceptor>));
                ilGenerator.DeclareLocal(typeof(MethodInfo));
                ilGenerator.DeclareLocal(typeof(Dictionary<string, object>));
                ilGenerator.DeclareLocal(typeof(InvocationInfo));
                ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.DeclareLocal(typeof(IInterceptor));
                ilGenerator.DeclareLocal(typeof(object));
                ilGenerator.DeclareLocal(typeof(object));
                ilGenerator.DeclareLocal(typeof(Type[]));

                //var cancel = ilGenerator.DefineLabel();
                var bypassAfter = ilGenerator.DefineLabel();
                var bypassBefore = ilGenerator.DefineLabel();
                var loop1Begin = ilGenerator.DefineLabel();
                var loop1IndexCheck = ilGenerator.DefineLabel();
                var loop2Begin = ilGenerator.DefineLabel();
                var loop2IndexCheck = ilGenerator.DefineLabel();
                var endOfMethod = ilGenerator.DefineLabel();
                var finish = ilGenerator.DefineLabel();

                ilGenerator.BeginExceptionBlock();

                /* Create Interceptor Cache */
                ilGenerator.Emit(OpCodes.Newobj, interceptorCache);
                ilGenerator.Emit(OpCodes.Stloc_0);
                /* End Of Create Interceptor Cache */

                #region Create Interceptor Instances 
                for (var index = 0; index < interceptors.Length; index++)
                {
                    var interceptor = interceptors[index].GetConstructor(Type.EmptyTypes);
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Newobj, interceptor);
                    ilGenerator.Emit(OpCodes.Call, addInterceptor);
                }
                #endregion End Of Create Interceptor Instances 

                #region Get info of base method 
                ilGenerator.Emit(OpCodes.Ldc_I4, methodParameters.Length);
                ilGenerator.Emit(OpCodes.Newarr, typeof(Type));
                ilGenerator.Emit(OpCodes.Stloc, 8);

                if (methodParameters.Any())
                {
                    for (var index = 0; index < methodParameters.Length; index++)
                    {
                        ilGenerator.Emit(OpCodes.Ldloc, 8);
                        ilGenerator.Emit(OpCodes.Ldc_I4, index);
                        ilGenerator.Emit(OpCodes.Ldtoken, methodParameters[index].ParameterType);
                        ilGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                }

                ilGenerator.Emit(OpCodes.Ldtoken, baseType);
                ilGenerator.Emit(OpCodes.Call, typeFromRuntimeHandle);
                ilGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);
                ilGenerator.Emit(OpCodes.Ldloc, 8);
                ilGenerator.Emit(OpCodes.Call, getMethod);
                ilGenerator.Emit(OpCodes.Stloc_1);
                #endregion End Of Get info of base method 

                #region Collect Input Arguments Of Base Method and Push Them Into A Dictionary
                /* Dictionary<string,object> oluştur ve değişkene ata */
                ilGenerator.Emit(OpCodes.Newobj, parameters);
                ilGenerator.Emit(OpCodes.Stloc_2);
                /* Dictionary<string,object> oluştur ve değişkene ata */

                for (var index = 0; index < methodParameters.Length; index++)
                {
                    /* Dictionary<string,object>'i (parameters) stack'a yükle */
                    ilGenerator.Emit(OpCodes.Ldloc_2);

                    /* parametre ismini stack a yükle */
                    ilGenerator.Emit(OpCodes.Ldstr, methodParameters[index].Name);

                    /* index + 1 deki parametreyi stack'a yükle (metoda geçilen parametreler) */
                    ilGenerator.Emit(OpCodes.Ldarg, index + 1);

                    //cast
                    ilGenerator.Emit(OpCodes.Box, methodParameters[index].ParameterType);

                    //Dictinary.Add(Tkey,TValue) metodunu çağır
                    ilGenerator.Emit(OpCodes.Call, addParameter);
                }
                #endregion Collect Input Arguments Of Base Method and Push Them Into A Dictionary

                #region Prepare The Invocation Info
                //Load current instance "this"
                ilGenerator.Emit(OpCodes.Ldarg_0);
                //Load current instance "this" again
                ilGenerator.Emit(OpCodes.Ldarg_0);
                //Load method info 
                ilGenerator.Emit(OpCodes.Ldloc_1);
                //Load arguments passed into the method
                ilGenerator.Emit(OpCodes.Ldloc_2);
                //Create the InvcationInfo Instance
                ilGenerator.Emit(OpCodes.Newobj, invocationInfo);
                //Save it into the variable at loc 3
                ilGenerator.Emit(OpCodes.Stloc_3);
                #endregion End Of Prepare The Invocation Info

                #region Interceptor Invocation Loop1
                // var index = 0 
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                // save the variable at loc 4
                ilGenerator.Emit(OpCodes.Stloc, 4);
                // Go to Index Check 
                ilGenerator.Emit(OpCodes.Br_S, loop1IndexCheck);

                /* Begin To Loop1 */
                ilGenerator.MarkLabel(loop1Begin);

                #region Invoke the "BeforeExecution" Method 
                //Load interceptor cache into memory
                ilGenerator.Emit(OpCodes.Ldloc_0);
                // Load index value into the stack
                ilGenerator.Emit(OpCodes.Ldloc, 4);
                //Load the interceptor at Current index into the stack
                ilGenerator.Emit(OpCodes.Call, getItem);
                ilGenerator.Emit(OpCodes.Stloc, 5);
                ilGenerator.Emit(OpCodes.Ldloc, 5);
                //Load invocation info into the stack
                ilGenerator.Emit(OpCodes.Ldloc_3);
                //Invoke the "BeforeExecution" method
                ilGenerator.Emit(OpCodes.Callvirt, before);

                #endregion End Of Invoke the "BeforeExecution" Method

                #region Load The Result Comes From Before Methods
                ilGenerator.Emit(OpCodes.Ldloc_3);
                ilGenerator.Emit(OpCodes.Call, getResult);
                ilGenerator.Emit(OpCodes.Stloc, 6);
                #endregion Load The Result Comes From Before Methods

                #region Evaluate the InvocationInfo 

                #region Check isCancelled Set
                ////Load invocation info into the stack
                ilGenerator.Emit(OpCodes.Ldloc_3);
                ////Load isCancelled field into the stack
                ilGenerator.Emit(OpCodes.Ldfld, isCancelled);
                //Load 1 (true) into the stack
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Ceq);
                //If isCancelled flag is set then jump to the finish
                ilGenerator.Emit(OpCodes.Brtrue, finish);

                //ilGenerator.Emit(OpCodes.Br, cancel);
                #endregion End Of Check isCancelled Set

                #region Check Bypass Set
                ////Load invocation info into the stack
                ilGenerator.Emit(OpCodes.Ldloc_3);
                ////Load isCancelled field into the stack
                ilGenerator.Emit(OpCodes.Ldfld, bypass);
                //Load 1 (true) into the stack
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Ceq);
                //If isCancelled flag is set then jump to the cancel
                ilGenerator.Emit(OpCodes.Brtrue_S, bypassBefore);
                #endregion End Of Check Bypass Set

                #endregion End Of Evaluate the InvocationInfo

                #region Increment the Index Value
                //Load index into stack
                ilGenerator.Emit(OpCodes.Ldloc, 4);
                //Load 1 into stack
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                //Add 1 to index
                ilGenerator.Emit(OpCodes.Add);
                //Store the new index value
                ilGenerator.Emit(OpCodes.Stloc, 4);

                #endregion End Of Increment the Index Value

                #region Loop1 Index Check 
                ilGenerator.MarkLabel(loop1IndexCheck);
                //Load Interceptor Cache Into Memory
                ilGenerator.Emit(OpCodes.Ldloc_0);
                //Count Of Interceptors in Cache
                ilGenerator.Emit(OpCodes.Call, getCount);
                // Load Index Value Into The Stack
                ilGenerator.Emit(OpCodes.Ldloc, 4);
                //Compare CurrentIndex And Interceptor Count In The Cache If Index Value Less Than Interceptor Count 
                ilGenerator.Emit(OpCodes.Ceq);
                //Then Jump To Beginning Of The Loop1
                ilGenerator.Emit(OpCodes.Brfalse, loop1Begin);
                #endregion End Of Loop1 Index Check

                #endregion End Of Interceptor Invocation Loop1

                ilGenerator.MarkLabel(bypassBefore);

                #region Invoke the Base Method
                //Load current instance "this"
                ilGenerator.Emit(OpCodes.Ldarg_0);
                //Load method parameter into the stack
                for (int index = 0; index < methodParameters.Length; index++)
                {
                    //load input parameter at index + 1 into the stack
                    ilGenerator.Emit(OpCodes.Ldarg, index + 1);
                }
                //Invoke the base method
                ilGenerator.Emit(OpCodes.Call, methodInfo);

                //If current method returns a value or instance
                if (methodInfo.ReturnType != typeof(void))
                {
                    //Store the return value
                    ilGenerator.Emit(OpCodes.Stloc, 6);
                    //Load InvocationInfo into stack
                    ilGenerator.Emit(OpCodes.Ldloc, 3);
                    //Load return value into stack
                    ilGenerator.Emit(OpCodes.Ldloc, 6);
                    //Then set the "Result" property on InvocationInfo
                    ilGenerator.Emit(OpCodes.Call, setResult);
                }
                #endregion Invoke the Base Method 

                #region Interceptor Invocation Loop2
                // var index = 0 
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                // save the variable at loc 4
                ilGenerator.Emit(OpCodes.Stloc, 4);
                // Go to Index Check 
                ilGenerator.Emit(OpCodes.Br_S, loop2IndexCheck);
                /* Begin To Loop2 */
                ilGenerator.MarkLabel(loop2Begin);

                #region Invoke the "AfterExecution" Method 
                //Load interceptor cache into memory
                ilGenerator.Emit(OpCodes.Ldloc_0);
                // Load index value into the stack
                ilGenerator.Emit(OpCodes.Ldloc, 4);
                //Load the interceptor at Current index into the stack
                ilGenerator.Emit(OpCodes.Call, getItem);
                ilGenerator.Emit(OpCodes.Stloc, 5);
                ilGenerator.Emit(OpCodes.Ldloc, 5);
                //Load invocation info into the stack
                ilGenerator.Emit(OpCodes.Ldloc_3);
                //Invoke the "BeforeExecution" method
                ilGenerator.Emit(OpCodes.Callvirt, after);
                #endregion End Of Invoke the "AfterExecution" Method

                #region Load The Result Comes From After Methods
                ilGenerator.Emit(OpCodes.Ldloc_3);
                ilGenerator.Emit(OpCodes.Call, getResult);
                ilGenerator.Emit(OpCodes.Stloc, 6);
                #endregion Load The Result Comes From After Methods

                #region Evaluate the InvocationInfo 

                #region Check Bypass Set
                //Load invocation info into the stack
                ilGenerator.Emit(OpCodes.Ldloc_3);
                //Load isCancelled field into the stack
                ilGenerator.Emit(OpCodes.Ldfld, bypass);
                //Load 1 (true) into the stack
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Ceq);
                //If isCancelled flag is set then jump to the cancel
                ilGenerator.Emit(OpCodes.Brtrue, bypassAfter);
                #endregion End Of Check Bypass Set

                #endregion End Of Evaluate the InvocationInfo

                #region Increment the Index Value
                //Load index into stack
                ilGenerator.Emit(OpCodes.Ldloc, 4);
                //Load 1 into stack
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                //Add 1 to index
                ilGenerator.Emit(OpCodes.Add);
                //Store the new index value
                ilGenerator.Emit(OpCodes.Stloc, 4);

                #endregion End Of Increment the Index Value

                #region Loop2 Index Check 
                ilGenerator.MarkLabel(loop2IndexCheck);
                //Load Interceptor Cache Into Memory
                ilGenerator.Emit(OpCodes.Ldloc_0);
                //Count Of Interceptors in Cache
                ilGenerator.Emit(OpCodes.Call, getCount);
                // Load Index Value Into The Stack
                ilGenerator.Emit(OpCodes.Ldloc, 4);
                //Compare CurrentIndex And Interceptor Count In The Cache If Index Value Less Than Interceptor Count 
                ilGenerator.Emit(OpCodes.Ceq);
                //Then Jump To Beginning Of The Loop2
                ilGenerator.Emit(OpCodes.Brfalse, loop2Begin);
                #endregion End Of Loop2 Index Check

                #endregion End Of Interceptor Invocation Loop2

                ilGenerator.MarkLabel(bypassAfter);
                ilGenerator.MarkLabel(finish);

                ilGenerator.Emit(OpCodes.Leave, endOfMethod);

                #region Catch Block

                ilGenerator.BeginCatchBlock(typeof(Exception));

                var exceptionHandler = config.GetExceptionHandler(methodInfo);

                if (exceptionHandler == null)
                {
                    exceptionHandler = typeof(DefaultExceptionHandler);
                }

                ilGenerator.DeclareLocal(typeof(Exception));
                ilGenerator.DeclareLocal(typeof(Type[]));
                ilGenerator.DeclareLocal(typeof(Type));
                ilGenerator.DeclareLocal(typeof(Type));
                ilGenerator.DeclareLocal(typeof(MethodInfo));
                ilGenerator.DeclareLocal(typeof(object));   //ExceptionHandler
                ilGenerator.DeclareLocal(typeof(object[]));

                //Store the exception instance
                ilGenerator.Emit(OpCodes.Stloc, 9);

                //Create an type array and store It
                ilGenerator.Emit(OpCodes.Ldc_I4_2);
                ilGenerator.Emit(OpCodes.Newarr, typeof(Type));
                ilGenerator.Emit(OpCodes.Stloc, 10);

                //Get exception type end store It
                ilGenerator.Emit(OpCodes.Ldloc, 9);
                ilGenerator.Emit(OpCodes.Call, getType);
                ilGenerator.Emit(OpCodes.Stloc, 11);

                //Set instance in array at index zero by exception type  
                ilGenerator.Emit(OpCodes.Ldloc, 10);
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Ldloc, 11);
                ilGenerator.Emit(OpCodes.Stelem_Ref);

                //Set instance in array at index zero by exception type  
                ilGenerator.Emit(OpCodes.Ldloc, 10);
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Ldtoken, typeof(IInvocationInfo));
                ilGenerator.Emit(OpCodes.Stelem_Ref);

                //Store the ExceptionHandler type 
                ilGenerator.Emit(OpCodes.Ldtoken, exceptionHandler);
                ilGenerator.Emit(OpCodes.Call, typeFromRuntimeHandle);
                ilGenerator.Emit(OpCodes.Stloc, 12);

                //Try to get OnException method override with current exception as input parameter and store It
                ilGenerator.Emit(OpCodes.Ldloc, 12);
                ilGenerator.Emit(OpCodes.Ldstr, "OnException");
                ilGenerator.Emit(OpCodes.Ldloc, 10);
                ilGenerator.Emit(OpCodes.Call, getMethod);
                ilGenerator.Emit(OpCodes.Stloc, 13);

                ilGenerator.DeclareLocal(typeof(MethodInfo));
                //Create the instance of ExceptionHandler and store It
                var exceptionHandlerCtor = exceptionHandler.GetConstructors().FirstOrDefault();
                ilGenerator.Emit(OpCodes.Newobj, exceptionHandlerCtor);
                ilGenerator.Emit(OpCodes.Stloc, 14);

                //Create and array for input parameters of ExceptionHandler specific override method
                ilGenerator.Emit(OpCodes.Ldc_I4_2);
                ilGenerator.Emit(OpCodes.Newarr, typeof(object));
                ilGenerator.Emit(OpCodes.Stloc, 15);

                //Set item at index zero by exception instance  
                ilGenerator.Emit(OpCodes.Ldloc, 15);
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Ldloc, 9);
                ilGenerator.Emit(OpCodes.Stelem_Ref);

                //Set item at index one by invocation info  
                ilGenerator.Emit(OpCodes.Ldloc, 15);
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Ldloc, 3);
                ilGenerator.Emit(OpCodes.Stelem_Ref);

                //Invoke the OnException method with the specific exception type as input parameter then jump to endOfCatch
                ilGenerator.Emit(OpCodes.Ldloc, 13);
                ilGenerator.Emit(OpCodes.Ldloc, 14);
                ilGenerator.Emit(OpCodes.Ldloc, 15);
                ilGenerator.Emit(OpCodes.Call, invoke);
                ilGenerator.Emit(OpCodes.Leave, endOfMethod);
                ilGenerator.EndExceptionBlock();

                #endregion End Of Catch Block

                ilGenerator.MarkLabel(endOfMethod);

                if (methodInfo.ReturnType != typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Ldloc_3);
                    ilGenerator.Emit(OpCodes.Call, getResult);
                }

                ilGenerator.Emit(OpCodes.Ret);

                _typeBuilder.DefineMethodOverride(derivedMethodBuilder, methodInfo);
            }
            OverrideConstructors();
            return _typeBuilder.CreateType();
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

                #region Prepare The Invocation Info
                //Load current instance "this"
                ilGenerator.Emit(OpCodes.Ldarg_0);

                for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
                {
                    //Load current instance "this"
                    ilGenerator.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
                //Create the InvcationInfo Instance
                #endregion End Of Prepare The Invocation Info

                ilGenerator.Emit(OpCodes.Call, constructorInfo);
                ilGenerator.Emit(OpCodes.Ret);
            }
        }
    }
}