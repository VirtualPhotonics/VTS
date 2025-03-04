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
            Assert.That(myTestClass.SettingsChanged, Is.True);
        }

        [Test]
        public void Test_HandleChangeTracking_changed()
        {
            var myTestClass = new BindableTestClass { First = 6 };
            myTestClass.Reset();
            Assert.That(myTestClass.SettingsChanged, Is.False);
            myTestClass.First = 0;
            Assert.That(myTestClass.First, Is.EqualTo(0));
            Assert.That(myTestClass.SettingsChanged, Is.True);
            myTestClass.First = 2;
            Assert.That(myTestClass.SettingsChanged, Is.True);
        }

        [Test]
        public void Test_HandleChangeTracking_unchanged()
        {
            var myTestClass = new BindableTestClass { First = 0 };
            Assert.That(myTestClass.First, Is.EqualTo(0));
            Assert.That(myTestClass.SettingsChanged, Is.True);
            myTestClass.First = 0;
            Assert.That(myTestClass.SettingsChanged, Is.False);
        }

        [Test]
        public void Test_Reset()
        {
            var myTestClass = new BindableTestClass { First = 0 };
            myTestClass.Reset();
            Assert.That(myTestClass.SettingsChanged, Is.False);
        }
    }
}
