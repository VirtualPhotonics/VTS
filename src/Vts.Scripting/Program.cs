// This is a top-level program that lets you run any of the demos in the Vts.Scripts project.
// It is a good place to start if you want to run a demo, or if you want to write your own code.
using Vts.Scripting;

// to run a single demo, find the one you want by browsing to them, and run it:
Vts.Scripting.ShortCourse.Demo02AnalogVsContinuousWithReflectance.RunDemo();
// or: Vts.Scripting.MonteCarlo.Demo01ROfRhoSimple.RunDemo();
// or: Vts.Scripting.ForwardSolvers.Demo01ROfRhoAndFtSingle.RunDemo();
// or: Vts.Scripting.ShortCourse.Demo01APhotonCountWithFluence.RunDemo();

// or, uncomment one of the following lines to run all demos in a category:
// BatchDemoRunner.RunAllMonteCarloDemos();
// BatchDemoRunner.RunAllForwardSolverDemos();
// BatchDemoRunner.RunAllShortCourseDemos();

// you can also put any of your own code here, if you want, e.g. commenting out this code will run a Monte Carlo simulation:
// var rhos = new DoubleRange(0.0, 10.0, 101);
// var simulationResults = new MonteCarloSimulation(
//     new SimulationInput
//     {
//         N = 10_000,
//         DetectorInputs = new IDetectorInput[] { new ROfRhoDetectorInput { Rho = rhos, Name = "ROfRho" } }
//     }).Run();
// var rhoMidpoints = rhos.GetMidpoints();
// var detectorResults = (ROfRhoDetector)simulationResults.ResultsDictionary["ROfRho"];
// LineChart(
//     xValues: rhoMidpoints, 
//     yValues: detectorResults.Mean.Select(r => Math.Log(r)).ToArray(), 
//     xLabel: "ρ [mm]", yLabel: "log(R(ρ) [mm-2])", title: "log(R(ρ))").Show();