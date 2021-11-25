
using ProxyFactoryCore.Tests.TestClasses;

namespace DynamicProxyCore.Tests
{
    public class Controller1
    {
        public string Index()
        {
            return "";
        }
        public int Add(ComplexType1 complexType1) 
        {
            return 1;
        }
        public int Add(string name, int age, float salary) 
        {

            return 1;
        }
    }

}
