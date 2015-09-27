using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SingleVoxelTissueInputTests
    {
        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new SingleVoxelTissueInput(
                new VoxelTissueRegion(
                    new DoubleRange(-1, 1), 
                    new DoubleRange(-1, 1),
                    new DoubleRange(1, 2),
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)), 
                new ITissueRegion[]
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

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mua, i.Regions[1].RegionOP.Mua);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new SingleVoxelTissueInput(
                new VoxelTissueRegion(
                    new DoubleRange(-1, 1),
                    new DoubleRange(-1, 1),
                    new DoubleRange(1, 2),
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)), 
               new ITissueRegion[]
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
            i.WriteToJson("SingleVoxelTissue.txt");
            var iCloned = FileIO.ReadFromJson<SingleVoxelTissueInput>("SingleVoxelTissue.txt");

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mua, i.Regions[1].RegionOP.Mua);
        }
    }
}
