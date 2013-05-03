%% RunSimulation
% Static method to run a single Monte Carlo simulation
%
%% Syntax
%  RunSimulation(SIMULATIONINPUT, WRITEDETECTORS)
%       SIMULATIONINPUT Simulation input class defining the data for 
%           the simulation.
%       WRITEDETECTORS Optional flag indicating whether to write detector
%           results to xml files
%
%% Description
% This static method runs a single Monte Carlo simulation given a
% SimulationInput class.  The user can optionally add a WRITEDETECTORS flag
% to also output the detector results to xml files.  This output is not
% necessary to view the results using these matlab scripts, but can be
% post-processed using the matlab scripts in
% matlab/post_processing/monte_carlo.
% 
%% Examples
%       % create a default set of inputs
%       si = SimulationInput();
%       % modify number of photons
%       si.N = 1000;
%       % specify a single R(rho) detector by the endpoints of rho bins
%       si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
%       % use this to run a Matlab-wrapped MonteCarloSimulation using static method
%       output = VtsMonteCarlo.RunSimulation(si);
%       d = output.Detectors(output.DetectorNames{1});
%       figure; semilogy(d.Rho, d.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
%
%% See Also
% <VtsMonteCarlo_help.html VtsMonteCarlo> | 
% <RunPostProcessor_help.html RunPostProcessor> | 
% <RunPostProcessors_help.html RunPostProcessors> | 
% <RunSimulations_help.html RunSimulations>
%%
% <html>
% <a href="matlab:doc SimulationInput">SimulationInput</a>
% </html>