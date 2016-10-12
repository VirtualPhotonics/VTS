%% ROfRho
% Reflectance as a function of rho
%
%% Syntax
%  ROfRho(OP, RHO) 
%   
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%
%% Description
% Returns reflectance as a function of source-detector separation
% (rho=sqrt(x*x+y*y))
%
%% Examples
%       op = [0.01 1 0.8 1.4]; % optical properties
%       rho = 0.5:0.05:9.5; %s-d separation, in mm
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfRho(op, rho);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <PHDOfRhoAndZTwoLayer_help.html PHDOfRhoAndZTwoLayer> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfFxTwoLayer_help.html ROfFxTwoLayer> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> |
% <ROfRhoAndFtTwoLayer_help.html ROfRhoAndFtTwoLayer> |
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <ROfRhoAndTimeTwoLayer_help.html ROfRhoAndTimeTwoLayer> |
% <ROfRhoTwoLayer_help.html ROfRhoTwoLayer> |
% <SetSolverType_help.html SetSolverType>