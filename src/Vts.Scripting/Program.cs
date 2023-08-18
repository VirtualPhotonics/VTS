using System.Runtime.CompilerServices;
using Vts.Scripting.ForwardSolvers;
using Vts.Scripting.MonteCarlo;

[assembly: InternalsVisibleTo("Vts.Scripting.Test")]
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
        FS20_ROfFxTwoLayerMultiOP.RunDemo();
    }

    /// <summary>
    /// Method to run all Monte Carlo demos
    /// </summary>
    internal static void RunAllMonteCarloDemos(bool showPlots = true)
    {
        MC01_ROfRhoSimple.RunDemo(showPlots);
        MC02_DAWvsCAW.RunDemo(showPlots);
        MC03_ROfRhoFullCustomization.RunDemo(showPlots);
        MC04_N1000vsN100.RunDemo(showPlots);
        MC05_PostProcessor.RunDemo(showPlots);
        MC06_pMCPostProcessor.RunDemo(showPlots);
        MC07_pMCInversion.RunDemo(showPlots);
        MC08_UnitTestComparison.RunDemo(showPlots);
        MC09_TransmittanceTallies.RunDemo(showPlots);
        MC10_ROfFx.RunDemo(showPlots);
    }

    /// <summary>
    /// Method to run all Forward Solver demos
    /// </summary>
    internal static void RunAllForwardSolverDemos(bool showPlots = true)
    {
        FS01_ROfRhoAndFtSingle.RunDemo(showPlots);
        FS02_ROfFxAndFtMulti.RunDemo(showPlots);
        FS03_FluenceOfRhoAndZ.RunDemo(showPlots);
        FS04_FluenceOfRhoAndZAndFt.RunDemo(showPlots);
        FS05_PHDOfRhoAndZ.RunDemo(showPlots);
        FS06_FluenceOfRhoAndZTwoLayer.RunDemo(showPlots);
        FS07_PHDOfRhoAndZTwoLayer.RunDemo(showPlots);
        FS08_AbsorbedEnergyOfRhoAndZ.RunDemo(showPlots);
        FS09_ROfRhoMulti.RunDemo(showPlots);
        FS10_ROfRhoAndTimeMulti.RunDemo(showPlots);
        FS11_ROfFxAndTime.RunDemo(showPlots);
        FS12_ROfFxSingle.RunDemo(showPlots);
        FS13_ROfFxMultiOP.RunDemo(showPlots);
        FS14_ROfFxMultiOPIntralipid.RunDemo(showPlots);
        FS15_ROfFxMultiOPMie.RunDemo(showPlots);
        FS16_ROfFxMultiPowerLaw.RunDemo(showPlots);
        FS17_ROfRhoMultiOP.RunDemo(showPlots);
        FS18_ROfRhoMultiOPInversion.RunDemo(showPlots);
        FS19_ROfRhoTwoLayerMultiOP.RunDemo(showPlots);
        FS20_ROfFxTwoLayerMultiOP.RunDemo(showPlots);
    }
}