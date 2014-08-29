using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.IO
{
    #region Dummy classes for tests below
    public class CompositeThingy
    {
        public IThingy OneThingy { get; set; }
        public IList<IThingy> SomeThingies { get; set; }
        public IThingy[] MoreThingies { get; set; }
        public FirstThingy[] FirstThingies { get; set; }
        public SecondThingy[] SecondThingies { get; set; }
    }

    public interface IThingy
    {
        string ThingyType { get; }
    }

    public enum ThingyType
    {
        First,
        Second
    }

    public class FirstThingy : IThingy
    {
        public string ThingyType { get { return "First"; } }
    }

    public class SecondThingy : IThingy
    {
        public string ThingyType { get { return "Second"; } }
    }
    #endregion

    [TestFixture]
    public class VtsJsonSerializerTests
    {
        /// <summary>
        /// clear all previously generated folders and files
        /// </summary>
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
            string[] fileList =
                {
                    "VtsJsonSerializerTests_file1.txt", 
                    "VtsJsonSerializerTests_file2.txt", 
                    "VtsJsonSerializerTests_file3.txt", 
                    "VtsJsonSerializerTests_file4.txt"
                };
            foreach (var file in fileList)
            {
                if (FileIO.FileExists(file))
                {
                    FileIO.FileDelete(file);
                }
            }
        }

        [Test]
        public void validate_file_exists_after_writing()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "World" }, "VtsJsonSerializerTests_file1.txt");
            Assert.IsTrue(FileIO.FileExists("VtsJsonSerializerTests_file1.txt"));
        }

        [Test]
        public void validate_deserialization_from_string()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new[] { "Hello", "Dolly" });
            var objectDeserialized = VtsJsonSerializer.ReadFromJson<string[]>(jsonSerialized);
            Assert.IsTrue(objectDeserialized != null);
            Assert.IsTrue(objectDeserialized.Length > 0);
            Assert.AreEqual(objectDeserialized[0], "Hello");
            Assert.AreEqual(objectDeserialized[1], "Dolly");
        }

        [Test]
        public void validate_deserialization_from_file()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "Sailor" }, "VtsJsonSerializerTests_file2.txt");
            var objectDeserialized = VtsJsonSerializer.ReadFromJsonFile<string[]>("VtsJsonSerializerTests_file2.txt");
            Assert.IsTrue(objectDeserialized != null);
            Assert.IsTrue(objectDeserialized.Length > 0);
            Assert.AreEqual(objectDeserialized[0], "Hello");
            Assert.AreEqual(objectDeserialized[1], "Sailor");
        }

        [Test]
        public void validate_deserialization_of_interface_object_from_string()
        {
            VtsJsonSerializer.KnownConverters.Add(ConventionBasedConverter<IThingy>.CreateFromEnum<ThingyType>(typeof(FirstThingy)));
            var compositeThingy = new CompositeThingy
            {
                OneThingy = new FirstThingy(),
                SomeThingies = new List<IThingy> { new FirstThingy(), new SecondThingy() },
                MoreThingies = new IThingy[] { new FirstThingy(), new SecondThingy() },
                FirstThingies = new [] { new FirstThingy(), new FirstThingy() },
                SecondThingies = new [] { new SecondThingy(), new SecondThingy() },

            };
            var jsonSerialized = VtsJsonSerializer.WriteToJson(compositeThingy);
            var thingyDeserialized = VtsJsonSerializer.ReadFromJson<CompositeThingy>(jsonSerialized);
            Assert.IsTrue(thingyDeserialized != null);
        }

        [Test]
        public void validate_deserialization_of_SimulationOptions()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationOptions());
            var optionsDerialized = VtsJsonSerializer.ReadFromJson<SimulationOptions>(jsonSerialized);
            Assert.IsTrue(optionsDerialized != null);
        }

        [Test]
        public void validate_serialization_of_SimulationInput()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationInput());
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_SimulationInput()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationInput());
            var inputDeserialized = VtsJsonSerializer.ReadFromJson<SimulationInput>(jsonSerialized);
            Assert.IsTrue(inputDeserialized != null);
        }

        [Test]
        public void validate_deserialization_of_SimulationInput_from_file()
        {
            VtsJsonSerializer.WriteToJsonFile(new SimulationInput(), "VtsJsonSerializerTests_file3.txt");
            var objectDeserialized = VtsJsonSerializer.ReadFromJsonFile<SimulationInput>("VtsJsonSerializerTests_file3.txt");
            Assert.IsTrue(objectDeserialized != null);
        }

        [Test]
        public void validate_serialization_of_IntRange()
        {
            var range = new IntRange(10, 20, 11);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(range);
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_IntRange()
        {
            var range = new IntRange(10, 20, 11);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(range);
            var intRangeDeserialized = VtsJsonSerializer.ReadFromJson<IntRange>(jsonSerialized);
            Assert.IsTrue(intRangeDeserialized != null);
            Assert.AreEqual(intRangeDeserialized.Start, 10);
            Assert.AreEqual(intRangeDeserialized.Stop, 20);
            Assert.AreEqual(intRangeDeserialized.Count, 11);
            Assert.AreEqual(intRangeDeserialized.Delta, 1);
        }

        [Test]
        public void validate_serialization_of_OpticalProperties()
        {
            var op = new OpticalProperties(0.011, 1.1, 0.99, 1.44);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(op);
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_OpticalProperties()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;
            var op = new OpticalProperties(0.011, 1.1, 0.99, 1.44);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(op);
            var opDeserialized = VtsJsonSerializer.ReadFromJson<OpticalProperties>(jsonSerialized);
            Assert.IsTrue(opDeserialized != null);
            Assert.IsTrue(areRoughlyEqual(opDeserialized.Mua, 0.011));
            Assert.IsTrue(areRoughlyEqual(opDeserialized.Musp, 1.1));
            Assert.IsTrue(areRoughlyEqual(opDeserialized.G, 0.99));
            Assert.IsTrue(areRoughlyEqual(opDeserialized.N, 1.44));
        }

        [Test]
        public void validate_serialization_of_LayerRegion()
        {
            var layer = new LayerRegion(
                zRange: new DoubleRange(3, 4, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var jsonSerialized = VtsJsonSerializer.WriteToJson(layer);

            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_LayerRegion()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;

            var layer = new LayerRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var jsonSerialized = VtsJsonSerializer.WriteToJson(layer);
            var layerDeserialized = VtsJsonSerializer.ReadFromJson<LayerRegion>(jsonSerialized);
            Assert.IsTrue(layerDeserialized != null);
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.ZRange.Start, 3.0));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.ZRange.Stop, 4.0));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.ZRange.Count, 2));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.Mua, 0.011));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.Musp, 1.1));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.G, 0.99));
            Assert.IsTrue(areRoughlyEqual(layerDeserialized.RegionOP.N, 1.44));
        }

        [Test]
        public void validate_serialization_of_MultiLayerTissueInput()
        {
            var layer0 = new LayerRegion(
                zRange: new DoubleRange(2.0, 3.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var layer1 = new LayerRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.0111, musp: 1.11, g: 0.999, n: 1.444));

            var multiRegionInput = new MultiLayerTissueInput(new[] { layer0, layer1 });

            var jsonSerialized = VtsJsonSerializer.WriteToJson(multiRegionInput);

            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        [Test]
        public void validate_deserialization_of_MultiLayerTissueInput()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;

            var layer0 = new LayerRegion(
                zRange: new DoubleRange(2.0, 3.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var layer1 = new LayerRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.0111, musp: 1.11, g: 0.999, n: 1.444));

            var multiRegionInput = new MultiLayerTissueInput(new[] {layer0, layer1});

            var jsonSerialized = VtsJsonSerializer.WriteToJson(multiRegionInput);
            var multiRegionInputDeserialized = VtsJsonSerializer.ReadFromJson<MultiLayerTissueInput>(jsonSerialized);
            Assert.IsTrue(multiRegionInputDeserialized != null);

            var region0Deserialized = (LayerRegion) multiRegionInputDeserialized.Regions[0];
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.ZRange.Start, 2.0));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.ZRange.Stop, 3.0));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.ZRange.Count, 2));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.Mua, 0.011));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.Musp, 1.1));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.G, 0.99));
            Assert.IsTrue(areRoughlyEqual(region0Deserialized.RegionOP.N, 1.44));

            var region1Deserialized = (LayerRegion)multiRegionInputDeserialized.Regions[1];
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.ZRange.Start, 3.0));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.ZRange.Stop, 4.0));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.ZRange.Count, 2));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.Mua, 0.0111));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.Musp, 1.11));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.G, 0.999));
            Assert.IsTrue(areRoughlyEqual(region1Deserialized.RegionOP.N, 1.444));
        }
        /// <summary>
        /// test to verify serialization and deserialization of gaussiansourceprofile runs successfully
        /// </summary>
        [Test]
        public void validate_serialization_and_deserialization_of_gaussiansourceprofile_runs_successfully()
        {
            var source = new Vts.MonteCarlo.CustomCircularSourceInput
            {
                SourceProfile = new GaussianSourceProfile(),
            };

            var sourceSerialized = VtsJsonSerializer.WriteToJson(source);

            var sourceDeserialized = VtsJsonSerializer.ReadFromJson<CustomCircularSourceInput>(sourceSerialized);

            Assert.IsTrue(sourceDeserialized.SourceProfile.SourceProfileType == SourceProfileType.Gaussian);
        }
    }
}
