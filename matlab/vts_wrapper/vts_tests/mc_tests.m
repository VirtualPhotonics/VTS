disp('Running unit tests for Monte Carlo simulations...');

% create a Matlab-wrapped SimulationInput DTO
si = SimulationInput;

% mutate an input option
si.N = 101;

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output = VtsMonteCarlo.RunSimulation(si);

disp('Done!');