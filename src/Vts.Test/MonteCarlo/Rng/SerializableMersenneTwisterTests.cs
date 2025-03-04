using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.MonteCarlo
{
    [TestFixture] 
    public class SerializableMersenneTwisterTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "savedRNG.txt"
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
        public void validate_saved_random_number_state_is_correct()
        {
            int seed = 0;
            // normal processing
            var rng = new SerializableMersenneTwister(seed);
            rng.NextDouble();
            rng.NextDouble();
            rng.NextDouble();
            // saved processing
            rng.ToFile(rng, "savedRNG.txt");
            var savedRNG = SerializableMersenneTwister.FromFile("savedRNG.txt");
            // saved processing next rng
            var savedRNG5 = savedRNG.NextDouble();
            // normal processing next rng
            var rng5 = rng.NextDouble();
            Assert.That(rng5 == savedRNG5, Is.True);
        }

    }

}
