%% PHDOfRhoAndZTwoLayer
% Photon Hitting Density in cylindrical coordinates for a two layer tissue 
% with specified source-detector separation and top layer thickness
%
%% Syntax
%  PHDOfRhoAndZTwoLayer(OP, RHOS, ZS, SD, LAYERTHICKNESS)
%
%       OP is an array of N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%       RHO is a 1 x M array of x values (in mm)
%           eg. RHO = linspace(0,10,10);
%       Z is a 1 x M array of z values (in mm)
%           eg. Z = linspace(0.1,19.9,100);
%       SD is the source-detector separation in mm
%       LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
%
%% Description
% Returns photon hitting density as a function of source-detector separation
% (rho = sqrt(x*x+y*y)) and depth (z).  Cylindrical symmetry centered
% about rho = 0 is assumed for a two layer tissue with top layer thickness
%
%% Examples
%       op = [[0.01 1 0.8 1.4];[0.1 1 0.8 1.4]];
%       rhos = linspace(0.1,19.9,100); % s-d separation, in mm
%       zs = linspace(0.1,19.9,100); % z range in mm
%       phd = VtsSolvers.PHDOfRhoAndZTwoLayer(op, rhos, zs, 10, 5);
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