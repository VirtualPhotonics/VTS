%% ROfFx
% Spatial-frequency reflectance 
%
%% Syntax
%  ROfFx(OP, FX) 
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       FX is an 1 x M array of spatial frequencies (in 1/mm)
%           eg. FX = linspace(0,0.5,11);
%
%% Description
% Returns steady-state reflectance in the spatial frequency domain
%
%% Examples
%       op = [0.1 1.2 0.8 1.4]; % optical properties
%       fx = 0:0.001:0.2; % range of spatial frequencies in 1/mm
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfFx(op, fx);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> | 
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>