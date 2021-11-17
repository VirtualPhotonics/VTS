using System;
using NUnit.Framework;

namespace Vts.Test.Common
{
    [TestFixture]
    class BindableObjectTests
    {
        [Test]
        public void Test_VerifyProperty_throws_exception()
        {
            var myTestClass = new BindableTestClass { First = 0 };
#if DEBUG
            Assert.Throws<ArgumentNullException>(() => myTestClass.OnPropertyChanged("NotAProperty"));
#else
            Assert.DoesNotThrow(() => myTestClass.OnPropertyChanged("NotAProperty"));
#endif
        }

        [Test]
        public void Test_GetPropertyChangedEventArgs_throws_exception()
        {
            Assert.Throws<ArgumentException>(() => BindableObject.GetPropertyChangedEventArgs(""));
        }
    }
}
