%% FluenceOfRhoAndZ 
% Fluence as a function of rho and z
%
%% Syntax
%  FluenceOfRhoAndZ(OP, RHOS, ZS) 
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
% Returns fluence as a function of source-detector separation (rho = sqrt(x*x+y*y)) and
% depth (z).  Cylindrical symmetry centered about rho = 0 is assumed.
%
%% Examples
%       op = [0.01, 1, 0.9, 1.4]; % optical properties 
%       rhos = 0.1:0.1:10; % s-d separation, in mm
%       zs = 0.1:0.1:10; % z range in mm
%       VtsSolvers.SetSolverType('DistributedGaussianSourceSDA')
%       fluence = VtsSolvers.FluenceOfRhoAndZ(op, rhos, zs);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> | 
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>