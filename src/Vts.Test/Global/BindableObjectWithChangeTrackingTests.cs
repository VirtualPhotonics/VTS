using NUnit.Framework;

namespace Vts.Test
{
    [TestFixture]
    internal class BindableObjectWithChangeTrackingTests
    {
        [Test]
        public void Test_SetProperty()
        {
            var myTestClass = new BindableTestClass { First = 6 };
            Assert.IsTrue(myTestClass.SettingsChanged);
        }

        [Test]
        public void Test_HandleChangeTracking_changed()
        {
            var myTestClass = new BindableTestClass { First = 6 };
            myTestClass.Reset();
            Assert.IsFalse(myTestClass.SettingsChanged);
            myTestClass.First = 0;
            Assert.AreEqual(0, myTestClass.First);
            Assert.IsTrue(myTestClass.SettingsChanged);
            myTestClass.First = 2;
            Assert.IsTrue(myTestClass.SettingsChanged);
        }

        [Test]
        public void Test_HandleChangeTracking_unchanged()
        {
            var myTestClass = new BindableTestClass { First = 0 };
            Assert.AreEqual(0, myTestClass.First);
            Assert.IsTrue(myTestClass.SettingsChanged);
            myTestClass.First = 0;
            Assert.IsFalse(myTestClass.SettingsChanged);
        }

        [Test]
        public void Test_Reset()
        {
            var myTestClass = new BindableTestClass { First = 0 };
            myTestClass.Reset();
            Assert.IsFalse(myTestClass.SettingsChanged);
        }
    }
}
