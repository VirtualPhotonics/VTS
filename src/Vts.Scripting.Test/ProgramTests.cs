namespace Vts.Scripting.Test;

public class ProgramTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Confirm_RunAllMonteCarloDemos_Does_Not_Throw()
    {
        Assert.DoesNotThrow(() => BatchDemoRunner.RunAllMonteCarloDemos(showPlots: false));
    }

    [Test]
    public void Confirm_RunAllForwardSolverDemos_Does_Not_Throw()
    {
        Assert.DoesNotThrow(() => BatchDemoRunner.RunAllForwardSolverDemos(showPlots: false));
    }

    [Test]
    public void Confirm_RunAllShortCourseDemos_Does_Not_Throw()
    {
        Assert.DoesNotThrow(() => BatchDemoRunner.RunAllShortCourseDemos(showPlots: false));
    }
}