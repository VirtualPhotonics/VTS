using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
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
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "VtsJsonSerializerTests_file1.txt", 
            "VtsJsonSerializerTests_file2.txt", 
            "VtsJsonSerializerTests_file3.txt", 
            "VtsJsonSerializerTests_file4.txt"
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        [Test]
        public void validate_file_exists_after_writing()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "World" }, "VtsJsonSerializerTests_file1.txt");
            Assert.That(FileIO.FileExists("VtsJsonSerializerTests_file1.txt"), Is.True);
        }

        [Test]
        public void validate_deserialization_from_string()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new[] { "Hello", "Dolly" });
            var objectDeserialized = VtsJsonSerializer.ReadFromJson<string[]>(jsonSerialized);
            Assert.That(objectDeserialized != null, Is.True);
            Assert.That(objectDeserialized.Length > 0, Is.True);
            Assert.That(objectDeserialized[0], Is.EqualTo("Hello"));
            Assert.That(objectDeserialized[1], Is.EqualTo("Dolly"));
        }

        [Test]
        public void validate_deserialization_from_file()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "Sailor" }, "VtsJsonSerializerTests_file2.txt");
            var objectDeserialized = VtsJsonSerializer.ReadFromJsonFile<string[]>("VtsJsonSerializerTests_file2.txt");
            Assert.That(objectDeserialized != null, Is.True);
            Assert.That(objectDeserialized.Length > 0, Is.True);
            Assert.That(objectDeserialized[0], Is.EqualTo("Hello"));
            Assert.That(objectDeserialized[1], Is.EqualTo("Sailor"));
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
            Assert.That(thingyDeserialized != null, Is.True);
        }

        [Test]
        public void validate_deserialization_of_SimulationOptions()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationOptions());
            var optionsDerialized = VtsJsonSerializer.ReadFromJson<SimulationOptions>(jsonSerialized);
            Assert.That(optionsDerialized != null, Is.True);
        }

        [Test]
        public void validate_serialization_of_SimulationInput()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationInput());
            Assert.That(jsonSerialized != null && jsonSerialized.Length > 0, Is.True);
        }

        [Test] 
        public void validate_deserialization_of_SimulationInput()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationInput());
            var inputDeserialized = VtsJsonSerializer.ReadFromJson<SimulationInput>(jsonSerialized);
            Assert.That(inputDeserialized != null, Is.True);
        }

        [Test]
        public void validate_deserialization_of_SimulationInput_from_file()
        {
            VtsJsonSerializer.WriteToJsonFile(new SimulationInput(), "VtsJsonSerializerTests_file3.txt");
            var objectDeserialized = VtsJsonSerializer.ReadFromJsonFile<SimulationInput>("VtsJsonSerializerTests_file3.txt");
            Assert.That(objectDeserialized != null, Is.True);
        }

        [Test]
        public void validate_serialization_of_IntRange()
        {
            var range = new IntRange(10, 20, 11);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(range);
            Assert.That(jsonSerialized != null && jsonSerialized.Length > 0, Is.True);
        }

        [Test]
        public void validate_deserialization_of_IntRange()
        {
            var range = new IntRange(10, 20, 11);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(range);
            var intRangeDeserialized = VtsJsonSerializer.ReadFromJson<IntRange>(jsonSerialized);
            Assert.That(intRangeDeserialized != null, Is.True);
            Assert.That(intRangeDeserialized.Start, Is.EqualTo(10));
            Assert.That(intRangeDeserialized.Stop, Is.EqualTo(20));
            Assert.That(intRangeDeserialized.Count, Is.EqualTo(11));
            Assert.That(intRangeDeserialized.Delta, Is.EqualTo(1));
        }

        [Test]
        public void validate_serialization_of_OpticalProperties()
        {
            var op = new OpticalProperties(0.011, 1.1, 0.99, 1.44);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(op);
            Assert.That(jsonSerialized != null && jsonSerialized.Length > 0, Is.True);
        }

        [Test]
        public void validate_deserialization_of_OpticalProperties()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;
            var op = new OpticalProperties(0.011, 1.1, 0.99, 1.44);
            var jsonSerialized = VtsJsonSerializer.WriteToJson(op);
            var opDeserialized = VtsJsonSerializer.ReadFromJson<OpticalProperties>(jsonSerialized);
            Assert.That(opDeserialized != null, Is.True);
            Assert.That(areRoughlyEqual(opDeserialized.Mua, 0.011), Is.True);
            Assert.That(areRoughlyEqual(opDeserialized.Musp, 1.1), Is.True);
            Assert.That(areRoughlyEqual(opDeserialized.G, 0.99), Is.True);
            Assert.That(areRoughlyEqual(opDeserialized.N, 1.44), Is.True);
        }

        [Test]
        public void validate_serialization_of_LayerRegion()
        {
            var layer = new LayerTissueRegion(
                zRange: new DoubleRange(3, 4, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var jsonSerialized = VtsJsonSerializer.WriteToJson(layer);

            Assert.That(jsonSerialized != null && jsonSerialized.Length > 0, Is.True);
        }

        [Test]
        public void validate_deserialization_of_LayerRegion()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;

            var layer = new LayerTissueRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var jsonSerialized = VtsJsonSerializer.WriteToJson(layer);
            var layerDeserialized = VtsJsonSerializer.ReadFromJson<LayerTissueRegion>(jsonSerialized);
            Assert.That(layerDeserialized != null, Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.ZRange.Start, 3.0), Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.ZRange.Stop, 4.0), Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.ZRange.Count, 2), Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.RegionOP.Mua, 0.011), Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.RegionOP.Musp, 1.1), Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.RegionOP.G, 0.99), Is.True);
            Assert.That(areRoughlyEqual(layerDeserialized.RegionOP.N, 1.44), Is.True);
        }

        [Test]
        public void validate_serialization_of_MultiLayerTissueInput()
        {
            var layer0 = new LayerTissueRegion(
                zRange: new DoubleRange(2.0, 3.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var layer1 = new LayerTissueRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.0111, musp: 1.11, g: 0.999, n: 1.444));

            var multiRegionInput = new MultiLayerTissueInput(new[] { layer0, layer1 });

            var jsonSerialized = VtsJsonSerializer.WriteToJson(multiRegionInput);

            Assert.That(jsonSerialized != null && jsonSerialized.Length > 0, Is.True);
        }

        [Test]
        public void validate_deserialization_of_MultiLayerTissueInput()
        {
            Func<double, double, bool> areRoughlyEqual = (a, b) => Math.Abs(a - b) < 0.001;

            var layer0 = new LayerTissueRegion(
                zRange: new DoubleRange(2.0, 3.0, 2),
                op: new OpticalProperties(mua: 0.011, musp: 1.1, g: 0.99, n: 1.44));

            var layer1 = new LayerTissueRegion(
                zRange: new DoubleRange(3.0, 4.0, 2),
                op: new OpticalProperties(mua: 0.0111, musp: 1.11, g: 0.999, n: 1.444));

            var multiRegionInput = new MultiLayerTissueInput(new[] {layer0, layer1});

            var jsonSerialized = VtsJsonSerializer.WriteToJson(multiRegionInput);
            var multiRegionInputDeserialized = VtsJsonSerializer.ReadFromJson<MultiLayerTissueInput>(jsonSerialized);
            Assert.That(multiRegionInputDeserialized != null, Is.True);

            var region0Deserialized = (LayerTissueRegion) multiRegionInputDeserialized.Regions[0];
            Assert.That(areRoughlyEqual(region0Deserialized.ZRange.Start, 2.0), Is.True);
            Assert.That(areRoughlyEqual(region0Deserialized.ZRange.Stop, 3.0), Is.True);
            Assert.That(areRoughlyEqual(region0Deserialized.ZRange.Count, 2), Is.True);
            Assert.That(areRoughlyEqual(region0Deserialized.RegionOP.Mua, 0.011), Is.True);
            Assert.That(areRoughlyEqual(region0Deserialized.RegionOP.Musp, 1.1), Is.True);
            Assert.That(areRoughlyEqual(region0Deserialized.RegionOP.G, 0.99), Is.True);
            Assert.That(areRoughlyEqual(region0Deserialized.RegionOP.N, 1.44), Is.True);

            var region1Deserialized = (LayerTissueRegion)multiRegionInputDeserialized.Regions[1];
            Assert.That(areRoughlyEqual(region1Deserialized.ZRange.Start, 3.0), Is.True);
            Assert.That(areRoughlyEqual(region1Deserialized.ZRange.Stop, 4.0), Is.True);
            Assert.That(areRoughlyEqual(region1Deserialized.ZRange.Count, 2), Is.True);
            Assert.That(areRoughlyEqual(region1Deserialized.RegionOP.Mua, 0.0111), Is.True);
            Assert.That(areRoughlyEqual(region1Deserialized.RegionOP.Musp, 1.11), Is.True);
            Assert.That(areRoughlyEqual(region1Deserialized.RegionOP.G, 0.999), Is.True);
            Assert.That(areRoughlyEqual(region1Deserialized.RegionOP.N, 1.444), Is.True);
        }
        /// <summary>
        /// test to verify serialization and deserialization of gaussiansourceprofile runs successfully
        /// </summary>
        [Test]
        public void validate_serialization_and_deserialization_of_gaussiansourceprofile_runs_successfully()
        {
            var source = new CustomCircularSourceInput
            {
                SourceProfile = new GaussianSourceProfile(),
            };

            var sourceSerialized = VtsJsonSerializer.WriteToJson(source);

            var sourceDeserialized = VtsJsonSerializer.ReadFromJson<CustomCircularSourceInput>(sourceSerialized);

            Assert.That(sourceDeserialized.SourceProfile.SourceProfileType == SourceProfileType.Gaussian, Is.True);
        }
    }
}
