using Vts.Scripting.ForwardSolvers;
using Vts.Scripting.MonteCarlo;

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
        //RunAllMonteCarloDemos();
        //RunAllForwardSolverDemos();

        MC06_pMCPostProcessor.RunDemo();
    }

    /// <summary>
    /// Method to run all Monte Carlo demos
    /// </summary>
    public static void RunAllMonteCarloDemos()
    {
        MC01_ROfRhoSimple.RunDemo();
        MC02_DAWvsCAW.RunDemo();
        MC03_ROfRhoFullCustomization.RunDemo();
        MC04_N1000vsN100.RunDemo();
        MC05_PostProcessor.RunDemo();
        MC06_pMCPostProcessor.RunDemo();
        MC07_pMCInversion.RunDemo();
        MC08_UnitTestComparison.RunDemo();
        MC09_TransmittanceTallies.RunDemo();
        MC10_ROfFx.RunDemo();
    }

    /// <summary>
    /// Method to run all Forward Solver demos
    /// </summary>
    public static void RunAllForwardSolverDemos()
    {
        FS01_ROfRhoAndFtSingle.RunDemo();
        FS02_ROfFxAndFtMulti.RunDemo();
        FS03_FluenceOfRhoAndZ.RunDemo();
        FS04_FluenceOfRhoAndZAndFt.RunDemo();
    }
}