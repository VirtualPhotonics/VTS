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
            var myTestClass = new MyTestClass { First = 0 };
            Assert.Throws<Exception>(() => myTestClass.OnPropertyChanged("NotAProperty"));
        }

        [Test]
        public void Test_GetPropertyChangedEventArgs_throws_exception()
        {
            var myTestClass = new MyTestClass { First = 0 };
            Assert.Throws<ArgumentException>(() => BindableObject.GetPropertyChangedEventArgs(""));
        }
    }
}
