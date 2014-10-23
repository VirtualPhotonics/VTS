% Unit tests for the Monte Carlo simulations
disp('Running unit tests for Monte Carlo simulations...');

% create a simple Matlab-wrapped SimulationInput DTO
si = SimulationInput();

% mutate an input option
si.N = 101;

% Test for detector input ROFAngle
si.DetectorInputs = { DetectorInput.ROfAngle(linspace(0,pi/2,2)) };
output = VtsMonteCarlo.RunSimulation(si);

% Test using alternate source inputs
si.SourceInput = SourceInput.CustomPoint(linspace(0, 0, 2), linspace(0, 0, 2), [0 0 0], [0 0 1], 0);
%si.SourceInput = SourceInput.IsotropicPoint([0 0 0], 0);
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
output = VtsMonteCarlo.RunSimulation(si);

% Test for detector input ROfRhoAndTime
si.DetectorInputs = { DetectorInput.ROfRhoAndTime(linspace(0,40,21), linspace(0,1,11), 'ROfRhoAndTime') };
output = VtsMonteCarlo.RunSimulation(si);

% Test for detector input FluenceOfRhoAndZAndTime
si.DetectorInputs = { DetectorInput.FluenceOfRhoAndZAndTime(linspace(0,40,21), linspace(0,10,11), linspace(0,1,11)) };
output = VtsMonteCarlo.RunSimulation(si);

% Test for detector input pMCROfFxAndTime
si.DetectorInputs = { DetectorInput.pMCROfFxAndTime(linspace(0,40,21), linspace(0,1,11)) };
output = VtsMonteCarlo.RunSimulation(si);

% Test for detector input ROfRhoAndOmega
si.DetectorInputs = { DetectorInput.ROfRhoAndOmega(linspace(0,10,11), linspace(0,1000,21)) };
output = VtsMonteCarlo.RunSimulation(si);

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
output = VtsMonteCarlo.RunSimulation(si);

% create and run a pMC-based MonteCarloSimulation
si = SimulationInput();
si.DetectorInputs = { DetectorInput.pMCROfRho(linspace(0,40,201)) };
si.Options.Databases = { 'pMCDiffuseReflectance' };
output = VtsMonteCarlo.RunSimulation(si);

% create and run a pMC-based MonteCarloSimulation
si = SimulationInput();
si.DetectorInputs = { DetectorInput.pMCROfRhoAndTime(linspace(0,40,21), linspace(0,1,11)) };
si.Options.Databases = { 'pMCDiffuseReflectance' };
output = VtsMonteCarlo.RunSimulation(si);

% create and run a MonteCarloSimulation w/ no on-the-fly tallies
si = SimulationInput();
si.DetectorInputs = { };
si.Options.Databases = { 'pMCDiffuseReflectance' };
output = VtsMonteCarlo.RunSimulation(si);

% create and run  post-processor based on the simulation above
ppi = PostProcessorInput();
ppi.DetectorInputs = {...
        DetectorInput.pMCROfRho(linspace(0,40,201))...
    };
ppi.TallySecondMoment = 0;
ppi.InputFolder = 'results';
ppi.DatabaseSimulationInputFilename = 'infile'; % unused if 2nd argument below is supplied
ppi.OutputName = 'ppresults';
output = VtsMonteCarlo.RunPostProcessor(ppi, output.Input);
% 
% %% plot the post-processed results
% 
% d = output.Detectors(output.DetectorNames{1});
% figure; semilogy(d.Rho, d.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');

% test the ability to run multiple simulations in parallel (todo: debug)
simInputs(1) = SimulationInput();
simInputs(1).N = 50;
simInputs(1).OutputName = 'results_test1'; % having non-overlapping names is critical
simInputs(2) = SimulationInput();
simInputs(2).N = 150;
simInputs(2).OutputName = 'results_test2'; % having non-overlapping names is critical
outputs = VtsMonteCarlo.RunSimulations(simInputs);

disp('Done!');
