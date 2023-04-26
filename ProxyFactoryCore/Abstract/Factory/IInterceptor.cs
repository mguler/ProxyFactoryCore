namespace ProxyFactoryCore.Abstract.Factory
{
    public interface IInterceptor
    {
        IInvocationInfo BeforeExecution(IInvocationInfo invocationInfo);
        IInvocationInfo AfterExecution(IInvocationInfo invocationInfo);
    }
}
