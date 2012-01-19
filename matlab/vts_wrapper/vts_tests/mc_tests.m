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

disp('Done!');