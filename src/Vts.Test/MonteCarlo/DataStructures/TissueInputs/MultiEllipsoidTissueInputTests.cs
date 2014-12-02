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
    public class MultiEllipsoidTissueInputTests
    {
        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new MultiEllipsoidTissueInput(new ITissueRegion[]
                    {
                        new EllipsoidTissueRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new EllipsoidTissueRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                    }, new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );

            var iCloned = i.Clone();

            Assert.AreEqual(iCloned.EllipsoidRegions[1].RegionOP.Mus, i.EllipsoidRegions[1].RegionOP.Mus);
            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new MultiEllipsoidTissueInput(new ITissueRegion[] 
                    {
                        new EllipsoidTissueRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new EllipsoidTissueRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                    }, new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );
            i.WriteToJson("MultiLayerTissue.txt");
            var iCloned = FileIO.ReadFromJson<MultiEllipsoidTissueInput>("MultiLayerTissue.txt");

            Assert.AreEqual(iCloned.EllipsoidRegions[1].RegionOP.Mus, i.EllipsoidRegions[1].RegionOP.Mus);
            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }
    }
}
