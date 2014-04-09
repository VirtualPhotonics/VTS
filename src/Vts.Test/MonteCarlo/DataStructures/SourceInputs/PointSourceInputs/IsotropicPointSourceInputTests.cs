using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class IsotropicPointSourceInputTests
    {
        
        [Test]
        public void validate_general_constructor_with_position()
        {      

            var position = new Position(1.0, 2.0, 3.0);

            var ps = new IsotropicPointSourceInput(position, 0)
            {                
            };
           
            Assert.IsTrue(
                ps.PointLocation.X == 1.0 &&
                ps.PointLocation.Y == 2.0 &&
                ps.PointLocation.Z == 3.0
           );
        }

        [Test]
        public void validate_default_constructor_with_position()
        {           
            var ps = new IsotropicPointSourceInput()
            {
            };

            Assert.IsTrue(
                ps.PointLocation.X == 0.0 &&
                ps.PointLocation.Y == 0.0 &&
                ps.PointLocation.Z == 0.0
           );
        }
    }
}
