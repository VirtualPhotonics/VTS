%% RunSimulations 
% Static method to run multiple Monte Carlo simulations
%
%% Syntax
%  RunSimulations(SIMULATIONINPUTS, WRITEDETECTORS)
%       SIMULATIONINPUTS List of simulation input classes defining the data for 
%           the simulation.
%       WRITEDETECTORS Optional flag indicating whether to write detector
%           results to xml files
%
%% Description
% This static method runs a multiple Monte Carlo simulations given a list
% of SimulationInput classes.  The user can optionally add a WRITEDETECTORS flag
% to also output the detector results to xml files.  This output is not
% necessary to view the results using these matlab scripts, but can be
% post-processed using the matlab scripts in
% matlab/post_processing/monte_carlo.
%
%% Examples
%       % create a list of two default SimulationInput with different numbers of 
%       % photons
%       si1 = SimulationInput();
%       % modify number of photons
%       si1.N = 1000;
%       si2 = SimulationInput();
%       s12.N = 100;
%       % specify a single R(rho) detector by the endpoints of rho bins
%       si1.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
%       si2.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
%       % create list of these 2 imput
%       si = [ si1; si2 ];
%       % use this to run a Matlab-wrapped MonteCarloSimulation using static method
%       output = VtsMonteCarlo.RunSimulations(si);
%       d1 = output{1}.Detectors(output{1}.DetectorNames{1});
%       figure; semilogy(d1.Rho, d1.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
%       d2 = output{2}.Detectors(output{2}.DetectorNames{1});
%       figure; semilogy(d2.Rho, d2.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
%
%% See Also
% <VtsMonteCarlo_help.html VtsMonteCarlo> | 
% <RunPostProcessor_help.html RunPostProcessor> | 
% <RunPostProcessors_help.html RunPostProcessors> | 
% <RunSimulation_help.html RunSimulation>
%%
% <html>
% <a href="matlab:doc SimulationInput">SimulationInput</a>
% </html>