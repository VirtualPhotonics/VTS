using System;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Extensions
{
    /// <summary>
    /// test check that bitmap definitions in Enums are setup correctly
    /// ref: http://msdn.microsoft.com/en-us/library/cc138362.aspx
    /// </summary>
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Flags]
        private enum Days
        {
            None = 0x0,
            Sunday = 0x1,
            Monday = 0x2,
            Tuesday = 0x4,
            Wednesday = 0x8,
            Thursday = 0x10,
            Friday = 0x20,
            Saturday = 0x40
        }

        [Test]
        public void Validate_HasFlag_returns_correct_values()
        {
            const Days meetingDays = Days.Tuesday | Days.Thursday;
            Assert.IsTrue(meetingDays.HasFlag(Days.Tuesday));
            Assert.IsFalse(meetingDays.HasFlag(Days.Monday));
        }

        [Test]
        public void Validate_Add_returns_correct_values()
        {
            var meetingDays = Days.Tuesday;
            meetingDays = meetingDays.Add(Days.Thursday);
            Assert.IsTrue(meetingDays == (Days.Tuesday | Days.Thursday));
        }

        [Test]
        public void Validate_Add_throws_exception()
        {
            const Days meetingDays = Days.Saturday;
            var range = new DoubleRange(0.0, 9.0, 10);
            Assert.Throws<ArgumentException>(() => meetingDays.Add(range));
        }

        [Test]
        public void Validate_Remove_returns_correct_values()
        {
            const Days meetingDays = Days.Tuesday | Days.Thursday;
            Assert.IsTrue(meetingDays.Remove(Days.Tuesday) == Days.Thursday);
        }

        [Test]
        public void Validate_Remove_returns_correct_values_if_not_set()
        {
            const Days meetingDays = Days.Thursday;
            var removed = meetingDays.Remove(Days.Tuesday);
            Assert.IsTrue(removed == Days.Thursday);
        }

        [Test]
        public void Validate_Remove_throws_exception()
        {
            const Days meetingDays = Days.Saturday;
            var range = new DoubleRange(0.0, 9.0, 10);
            Assert.Throws<ArgumentException>(() => meetingDays.Remove(range));
        }

        [Test]
        public void Validate_GetInternationalizedString_returns_string()
        {
            const ChromophoreCoefficientType enumToStringify = ChromophoreCoefficientType.FractionalAbsorptionCoefficient;
            Assert.AreEqual("vol. frac.", enumToStringify.GetInternationalizedString());
        }

        [Test]
        public void Validate_GetInternationalizedString_returns_empty_string()
        {
            const Days enumToStringify = Days.Saturday;
            Assert.AreEqual("", enumToStringify.GetInternationalizedString());
        }
    }
}
