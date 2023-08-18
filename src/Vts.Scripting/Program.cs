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
        Demo21ROfRhoAndTimeTwoLayerMultiOP.RunDemo();
    }

    /// <summary>
    /// Method to run all Monte Carlo demos
    /// </summary>
    internal static void RunAllMonteCarloDemos(bool showPlots = true)
    {
        Demo01ROfRhoSimple.RunDemo(showPlots);
        Demo02DAWvsCAW.RunDemo(showPlots);
        Demo03ROfRhoFullCustomization.RunDemo(showPlots);
        Demo04N1000vsN100.RunDemo(showPlots);
        Demo05PostProcessor.RunDemo(showPlots);
        Demo06pMCPostProcessor.RunDemo(showPlots);
        Demo07pMCInversion.RunDemo(showPlots);
        Demo08UnitTestComparison.RunDemo(showPlots);
        Demo09TransmittanceTallies.RunDemo(showPlots);
        Demo10ROfFx.RunDemo(showPlots);
    }

    /// <summary>
    /// Method to run all Forward Solver demos
    /// </summary>
    internal static void RunAllForwardSolverDemos(bool showPlots = true)
    {
        Demo01ROfRhoAndFtSingle.RunDemo(showPlots);
        Demo02ROfFxAndFtMulti.RunDemo(showPlots);
        Demo03FluenceOfRhoAndZ.RunDemo(showPlots);
        Demo04FluenceOfRhoAndZAndFt.RunDemo(showPlots);
        Demo05PHDOfRhoAndZ.RunDemo(showPlots);
        Demo06FluenceOfRhoAndZTwoLayer.RunDemo(showPlots);
        Demo07PHDOfRhoAndZTwoLayer.RunDemo(showPlots);
        Demo08AbsorbedEnergyOfRhoAndZ.RunDemo(showPlots);
        Demo09ROfRhoMulti.RunDemo(showPlots);
        Demo10ROfRhoAndTimeMulti.RunDemo(showPlots);
        Demo11ROfFxAndTime.RunDemo(showPlots);
        Demo12ROfFxSingle.RunDemo(showPlots);
        Demo13ROfFxMultiOP.RunDemo(showPlots);
        Demo14ROfFxMultiOPIntralipid.RunDemo(showPlots);
        Demo15ROfFxMultiOPMie.RunDemo(showPlots);
        Demo16ROfFxMultiPowerLaw.RunDemo(showPlots);
        Demo17ROfRhoMultiOP.RunDemo(showPlots);
        Demo18ROfRhoMultiOPInversion.RunDemo(showPlots);
        Demo19ROfRhoTwoLayerMultiOP.RunDemo(showPlots);
        Demo20ROfFxTwoLayerMultiOP.RunDemo(showPlots);
        Demo21ROfRhoAndTimeTwoLayerMultiOP.RunDemo(showPlots);
    }
}