using System.ComponentModel;
using NUnit.Framework;

namespace Vts.Test.Common
{
    public class MyTestClass : BindableObjectWithChangeTracking
    {
        private int _first;
        private int _third;

        public int First
        {
            get => _first;
            set
            {
                HandleChangeTracking("First", ref _first, ref _first);
                SetProperty("First", ref _first, ref value);
                _first = value;
            }
        }

        public int Second { get; set; }

        [DependsOn("Second")]
        public int Third
        {
            get => _third;
            set
            {
                _third = value;
                OnPropertyChanged("Second");
            }
        }

        public bool PropertyWasChanged { get; set; }

        public MyTestClass()
        {
            PropertyChanged += myTestClass_PropertyChanged;
        }

        private void myTestClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyWasChanged = true;
        }
    }

    [TestFixture]
    internal class BindableObjectWithChangeTrackingTests
    {
        [Test]
        public void Test_SetProperty()
        {
            var myTestClass = new MyTestClass { First = 6 };
            Assert.IsTrue(myTestClass.SettingsChanged);
        }

        [Test]
        public void Test_HandleChangeTracking_changed()
        {
            var myTestClass = new MyTestClass { First = 6 };
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
            var myTestClass = new MyTestClass { First = 0 };
            Assert.AreEqual(0, myTestClass.First);
            Assert.IsTrue(myTestClass.SettingsChanged);
            myTestClass.First = 0;
            Assert.IsFalse(myTestClass.SettingsChanged);
        }

        [Test]
        public void Test_Reset()
        {
            var myTestClass = new MyTestClass { First = 0 };
            myTestClass.Reset();
            Assert.IsFalse(myTestClass.SettingsChanged);
        }
    }
}
