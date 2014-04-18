using System.IO;
using System.Runtime.Serialization;
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
                        new EllipsoidRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new EllipsoidRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                    }, new ITissueRegion[]
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

            var iCloned = Clone(i);

            Assert.AreEqual(iCloned.EllipsoidRegions[1].RegionOP.Mus, i.EllipsoidRegions[1].RegionOP.Mus);
            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new MultiEllipsoidTissueInput(new ITissueRegion[] 
                    {
                        new EllipsoidRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new EllipsoidRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                    }, new ITissueRegion[]
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
            i.WriteToXML("MultiLayerTissue.xml");
            var iCloned = FileIO.ReadFromXML<MultiEllipsoidTissueInput>("MultiLayerTissue.xml");

            Assert.AreEqual(iCloned.EllipsoidRegions[1].RegionOP.Mus, i.EllipsoidRegions[1].RegionOP.Mus);
            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }

        private static T Clone<T>(T myObject)
        {
            using (MemoryStream ms = new MemoryStream(1024))
            {
                var dcs = new DataContractSerializer(typeof(T));
                dcs.WriteObject(ms, myObject);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }
    }
}
