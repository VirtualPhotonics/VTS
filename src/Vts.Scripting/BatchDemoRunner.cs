[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Vts.Scripting.Test")]
namespace Vts.Scripting;

/// <summary>
/// Static class to run all demos
/// </summary>
internal static class BatchDemoRunner
{
    /// <summary>
    /// Method to run all Monte Carlo demos
    /// </summary>
    internal static void RunAllMonteCarloDemos(bool showPlots = true)
    {
        MonteCarlo.Demo01ROfRhoSimple.RunDemo(showPlots);
        MonteCarlo.Demo02DawVsCaw.RunDemo(showPlots);
        MonteCarlo.Demo03ROfRhoFullCustomization.RunDemo(showPlots);
        MonteCarlo.Demo04N1000VsN100.RunDemo(showPlots);
        MonteCarlo.Demo05PostProcessor.RunDemo(showPlots);
        MonteCarlo.Demo06PmcPostProcessor.RunDemo(showPlots);
        MonteCarlo.Demo07PmcInversion.RunDemo(showPlots);
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
        ForwardSolvers.Demo05PhdOfRhoAndZ.RunDemo(showPlots);
        ForwardSolvers.Demo06FluenceOfRhoAndZTwoLayer.RunDemo(showPlots);
        ForwardSolvers.Demo07PhdOfRhoAndZTwoLayer.RunDemo(showPlots);
        ForwardSolvers.Demo08AbsorbedEnergyOfRhoAndZ.RunDemo(showPlots);
        ForwardSolvers.Demo09ROfRhoMulti.RunDemo(showPlots);
        ForwardSolvers.Demo10ROfRhoAndTimeMulti.RunDemo(showPlots);
        ForwardSolvers.Demo11ROfFxAndTime.RunDemo(showPlots);
        ForwardSolvers.Demo12ROfFxSingle.RunDemo(showPlots);
        ForwardSolvers.Demo13ROfFxMultiOpProp.RunDemo(showPlots);
        ForwardSolvers.Demo14ROfFxMultiOpPropIntralipid.RunDemo(showPlots);
        ForwardSolvers.Demo15ROfFxMultiOpPropMie.RunDemo(showPlots);
        ForwardSolvers.Demo16ROfFxMultiPowerLaw.RunDemo(showPlots);
        ForwardSolvers.Demo17ROfRhoMultiOpProp.RunDemo(showPlots);
        ForwardSolvers.Demo18ROfRhoMultiOpPropInversion.RunDemo(showPlots);
        ForwardSolvers.Demo19ROfRhoTwoLayerMultiOpProp.RunDemo(showPlots);
        ForwardSolvers.Demo20ROfFxTwoLayerMultiOpProp.RunDemo(showPlots);
        ForwardSolvers.Demo21ROfRhoAndFtTwoLayerMultiOpProp.RunDemo(showPlots);
        ForwardSolvers.Demo22ROfRhoAndFtTwoLayerMultiOpProp.RunDemo(showPlots);
    }

    /// <summary>
    /// Method to run all Short Course examples
    /// </summary>
    internal static void RunAllShortCourseDemos(bool showPlots = true)
    {
        ShortCourse.Demo01APhotonCountWithFluence.RunDemo(showPlots);
        ShortCourse.Demo01BAnalogVsDiscreteWithFluence.RunDemo(showPlots);
        ShortCourse.Demo02AnalogVsContinuousWithReflectance.RunDemo(showPlots);
    }
}