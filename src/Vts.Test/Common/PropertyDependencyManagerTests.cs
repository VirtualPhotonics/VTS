using NUnit.Framework;

namespace Vts.Test.Common
{
    [TestFixture]
    internal class PropertyDependencyManagerTests
    {
        [Test]
        public void Test_Register()
        {
            var myTestClass = new BindableTestClass();
            PropertyDependencyManager.Register(myTestClass);
            Assert.IsInstanceOf<BindableObjectWithChangeTracking>(myTestClass);
        }

        [Test]
        public void Test_PropertyChanged()
        {
            var myTestClass = new BindableTestClass();
            PropertyDependencyManager.Register(myTestClass);
            myTestClass.Third = 8;
            Assert.AreEqual(2, myTestClass.PropertyChangedCount);
        }
    }
}
