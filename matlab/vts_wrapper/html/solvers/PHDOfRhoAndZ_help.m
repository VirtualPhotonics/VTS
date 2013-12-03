%% PHDOfRhoAndZ
% Photon Hitting Density in cylindrical coordinates
%
%% Syntax
%  PHDOfRhoAndZ(OP, RHOS, ZS, SD)
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%       Z is a 1 x M array of z values (in mm)
%           eg. Z = linspace(0.1,19.9,100);
%       SD is the source-detector separation in mm
%
%% Description
% Returns photon hitting density as a function of source-detector separation
% (rho = sqrt(x*x+y*y)) and depth (z).  Cylindrical symmetry centered
% about rho = 0 is assumed.
%
%% Examples
%       op = [0.01 1 0.8 1.4]; % optical properties
%       rhos = linspace(0.1,19.9,100); % s-d separation, in mm
%       zs = linspace(0.1,19.9,100); % z range in mm
%       VtsSolvers.SetSolverType('DistributedGaussianSourceSDA');
%       phd = VtsSolvers.PHDOfRhoAndZ(op, rhos, zs, 10);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> | 
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>