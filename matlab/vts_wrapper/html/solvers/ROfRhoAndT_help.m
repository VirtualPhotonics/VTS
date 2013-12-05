%% ROfRhoAndT
% Reflectance as a function of rho and time
%
%% Syntax
%  ROfRhoAndT(OP, RHO, T) 
%   
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%       T is an 1 x O array of times (in ns)
%           eg. T = [1:10];
%
%% Description
% Returns reflectance as a function of source-detector separation
% (rho = sqrt(x*x+y*y)) and time (t)
%
%% Examples
%       op = [0.1 1.2 0.8 1.4]; % optical properties
%       rho = 10; %s-d separation, in mm
%       t = 0:0.001:0.5; % range of times in ns     
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfRhoAndT(op, rho, t);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> |
% <SetSolverType_help.html SetSolverType>