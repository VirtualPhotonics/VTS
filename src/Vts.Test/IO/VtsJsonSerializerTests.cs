using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.IO
{
    [TestFixture]
    public class VtsJsonSerializerTests
    {
        /// <summary>
        /// clear all previously generated folders and files
        /// </summary>
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
            if(FileIO.FileExists("file1.txt"))
            {
                FileIO.FileDelete("file1.txt");
            }
        }

        [Test]
        public void validate_file_exists_after_writing()
        {
            VtsJsonSerializer.WriteToJsonFile(new[]{ "Hello", "World" }, "file1.txt");
            Assert.IsTrue(FileIO.FileExists("file1.txt"));
        }

        [Test]
        public void validate_deserialization_from_string()
        {
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new[] { "Hello", "Dolly" });
            var objectDerialized = VtsJsonSerializer.ReadFromJson<string[]>(jsonSerialized);
            Assert.IsTrue(objectDerialized != null);
            Assert.IsTrue(objectDerialized.Length > 0);
            Assert.AreEqual(objectDerialized[0], "Hello");
            Assert.AreEqual(objectDerialized[1], "Dolly");
        }

        [Test]
        public void validate_deserialization_from_file()
        {
            VtsJsonSerializer.WriteToJsonFile(new[] { "Hello", "Sailor" }, "file2.txt");
            var objectDerialized = VtsJsonSerializer.ReadFromJsonFile<string[]>("file2.txt");
            Assert.IsTrue(objectDerialized != null);
            Assert.IsTrue(objectDerialized.Length > 0);
            Assert.AreEqual(objectDerialized[0], "Hello");
            Assert.AreEqual(objectDerialized[1], "Sailor");
        }

        [Test]
        public void validate_deserialization_of_interface_object_from_string()
        {
            VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<ThingyType, IThingy>(typeof(FirstThingy)));
            var compositeThingy = new CompositeThingy
            {
                OneThingy = new FirstThingy(),
                SomeThingies = new List<IThingy> { new FirstThingy(), new SecondThingy() },
                MoreThingies = new IThingy[] { new FirstThingy(), new SecondThingy() },
                FirstThingies = new [] { new FirstThingy(), new FirstThingy() },
                SecondThingies = new [] { new SecondThingy(), new SecondThingy() },

            };
            var jsonSerialized = VtsJsonSerializer.WriteToJson(compositeThingy);
            var thingyDerialized = VtsJsonSerializer.ReadFromJson<CompositeThingy>(jsonSerialized);
            Assert.IsTrue(thingyDerialized != null);
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
            VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.SourceType, ISourceInput>(typeof(IsotropicPointSourceInput)));
            VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.TissueType, ITissueInput>(typeof(MultiLayerTissueInput)));
            VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.TissueRegionType, ITissueRegion>(typeof(VoxelRegion), "Region"));
            VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.TallyType, IDetectorInput>(typeof(ROfRhoDetectorInput)));
            var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationInput());
            Assert.IsTrue(jsonSerialized != null && jsonSerialized.Length > 0);
        }

        //[Test]
        //public void validate_deserialization_of_SimulationInput()
        //{
        //    VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.SourceType, ISourceInput>(typeof(IsotropicPointSourceInput)));
        //    VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.TissueType, ITissueInput>(typeof(MultiLayerTissueInput)));
        //    VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.TissueRegionType, ITissueRegion>(typeof(VoxelRegion), "Region"));
        //    VtsJsonSerializer.KnownConverters.Add(new ConventionBasedConverter<Vts.MonteCarlo.TallyType, IDetectorInput>(typeof(ROfRhoDetectorInput)));
        //    var jsonSerialized = VtsJsonSerializer.WriteToJson(new SimulationInput());
        //    var inputDerialized = VtsJsonSerializer.ReadFromJson<SimulationInput>(jsonSerialized);
        //    Assert.IsTrue(inputDerialized != null);
        //}
    }

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
}
