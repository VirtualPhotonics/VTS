disp('Running unit tests for Monte Carlo simulations...');

% create a simple Matlab-wrapped SimulationInput DTO
si = SimulationInput();

% mutate an input option
si.N = 101;

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
output = VtsMonteCarlo.RunSimulation(si);

% create and run a pMC-based MonteCarloSimulation
si = SimulationInput();
si.DetectorInputs = { DetectorInput.pMCROfRho(linspace(0,40,201)) };
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

disp('Done!');