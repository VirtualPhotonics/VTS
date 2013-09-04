%% ROfFxAndT
% Reflectance as a function of spatial-frequency and time
%
%% Syntax
%  ROfFxAndT(OP, FX, T) 
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%       FX is an 1 x M array of spatial frequencies (in 1/mm)
%           eg. FX = linspace(0,0.5,11);
%       T is an 1 x O array of times (in ns)
%           eg. T = [1:10];
%
%% Description
% Returns reflectance as a function of spatial-frequency (fx) and time (t).
%
%% Examples
%       op = [0.1 1 0.8 1.4]; % optical properties
%       fx = linspace(0,0.5,11); % range of spatial frequencies in 1/mm
%       t = linspace(0,0.05,501); % range of times in ns
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfFxAndT(op, fx, t);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> | 
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>