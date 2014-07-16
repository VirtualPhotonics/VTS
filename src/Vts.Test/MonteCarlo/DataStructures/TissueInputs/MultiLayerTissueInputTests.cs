using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class MultiLayerTissueInputTests
    {
        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new MultiLayerTissueInput(new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );

            var iCloned = i.Clone();

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new MultiLayerTissueInput(new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );
            i.WriteToJson("MultiLayerTissue.txt");
            var iCloned = FileIO.ReadFromJson<MultiLayerTissueInput>("MultiLayerTissue.txt");

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }
    }
}
