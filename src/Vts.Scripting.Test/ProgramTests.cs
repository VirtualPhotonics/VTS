namespace Vts.Scripting.Test
{
    public class ProgramTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void confirm_RunAllMonteCarloDemos_does_not_throw()
        {
            // arrange
            TestDelegate testDelegate = Program.RunAllMonteCarloDemos;

            // assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void confirm_RunAllForwardSolverDemos_does_not_throw()
        {
            // arrange
            TestDelegate testDelegate = Program.RunAllForwardSolverDemos;

            // assert
            Assert.DoesNotThrow(testDelegate);
        }
    }
}