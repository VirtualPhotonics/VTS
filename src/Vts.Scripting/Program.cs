[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Vts.Scripting.Test")]
namespace Vts.Scripting;

/// <summary>
/// Top program used to execute the desired demo script(s)
/// </summary>
public class Program
{
    /// <summary>
    /// Main function that executes the desired demo script(s)
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        ForwardSolvers.Demo22ROfRhoAndFtTwoLayerMultiOP.RunDemo();
    }

    /// <summary>
    /// Method to run all Monte Carlo demos
    /// </summary>
    internal static void RunAllMonteCarloDemos(bool showPlots = true)
    {
        MonteCarlo.Demo01ROfRhoSimple.RunDemo(showPlots);
        MonteCarlo.Demo02DAWvsCAW.RunDemo(showPlots);
        MonteCarlo.Demo03ROfRhoFullCustomization.RunDemo(showPlots);
        MonteCarlo.Demo04N1000vsN100.RunDemo(showPlots);
        MonteCarlo.Demo05PostProcessor.RunDemo(showPlots);
        MonteCarlo.Demo06pMCPostProcessor.RunDemo(showPlots);
        MonteCarlo.Demo07pMCInversion.RunDemo(showPlots);
        MonteCarlo.Demo08UnitTestComparison.RunDemo(showPlots);
        MonteCarlo.Demo09TransmittanceTallies.RunDemo(showPlots);
        MonteCarlo.Demo10ROfFx.RunDemo(showPlots);
    }

    /// <summary>
    /// Method to run all Forward Solver demos
    /// </summary>
    internal static void RunAllForwardSolverDemos(bool showPlots = true)
    {
        ForwardSolvers.Demo01ROfRhoAndFtSingle.RunDemo(showPlots);
        ForwardSolvers.Demo02ROfFxAndFtMulti.RunDemo(showPlots);
        ForwardSolvers.Demo03FluenceOfRhoAndZ.RunDemo(showPlots);
        ForwardSolvers.Demo04FluenceOfRhoAndZAndFt.RunDemo(showPlots);
        ForwardSolvers.Demo05PHDOfRhoAndZ.RunDemo(showPlots);
        ForwardSolvers.Demo06FluenceOfRhoAndZTwoLayer.RunDemo(showPlots);
        ForwardSolvers.Demo07PHDOfRhoAndZTwoLayer.RunDemo(showPlots);
        ForwardSolvers.Demo08AbsorbedEnergyOfRhoAndZ.RunDemo(showPlots);
        ForwardSolvers.Demo09ROfRhoMulti.RunDemo(showPlots);
        ForwardSolvers.Demo10ROfRhoAndTimeMulti.RunDemo(showPlots);
        ForwardSolvers.Demo11ROfFxAndTime.RunDemo(showPlots);
        ForwardSolvers.Demo12ROfFxSingle.RunDemo(showPlots);
        ForwardSolvers.Demo13ROfFxMultiOP.RunDemo(showPlots);
        ForwardSolvers.Demo14ROfFxMultiOPIntralipid.RunDemo(showPlots);
        ForwardSolvers.Demo15ROfFxMultiOPMie.RunDemo(showPlots);
        ForwardSolvers.Demo16ROfFxMultiPowerLaw.RunDemo(showPlots);
        ForwardSolvers.Demo17ROfRhoMultiOP.RunDemo(showPlots);
        ForwardSolvers.Demo18ROfRhoMultiOPInversion.RunDemo(showPlots);
        ForwardSolvers.Demo19ROfRhoTwoLayerMultiOP.RunDemo(showPlots);
        ForwardSolvers.Demo20ROfFxTwoLayerMultiOP.RunDemo(showPlots);
        ForwardSolvers.Demo21ROfRhoAndFtTwoLayerMultiOP.RunDemo(showPlots);
        ForwardSolvers.Demo22ROfRhoAndFtTwoLayerMultiOP.RunDemo(showPlots);
    }
}