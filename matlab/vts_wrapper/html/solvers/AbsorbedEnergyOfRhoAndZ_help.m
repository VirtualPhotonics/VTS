%% AbsorbedEnergyOfRhoAndZ
% Absorbed Energy as a function of rho and z
%
%% Syntax
%  AbsorbedEnergyOfRhoAndZ(OP, RHOS, ZS)
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%       Z is a 1 x M array of z values (in mm)
%           eg. Z = linspace(0.1,19.9,100);
%
%% Description
% Returns absorbed energy as a function of source-detector separation
% (rho = sqrt(x*x+y*y)) and depth (z).
%
%% Examples
%       op = [0.01 1 0.8 1.4]; % optical properties
%       rhos = linspace(0.1,19.9,100); % s-d separation, in mm
%       zs = linspace(0.1,19.9,100); % z range in mm
%       VtsSolvers.SetSolverType('PointSourceSDA');
%       ae = VtsSolvers.AbsorbedEnergyOfRhoAndZ(op, rhos, zs);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> | 
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>