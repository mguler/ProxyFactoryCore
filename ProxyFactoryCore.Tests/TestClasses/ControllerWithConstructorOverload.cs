namespace ProxyFactoryCore.Tests.TestClasses
{
    public interface IInterface {
        void Test();
    }
    public class ControllerWithConstructorOverload:IInterface
    {
        public ControllerWithConstructorOverload(ComplexType1 complexType1,int p2, ComplexType2 complexType2,string t)
        {

        }

        public void Test()
        {
            throw new System.NotImplementedException();
        }
    }
}
