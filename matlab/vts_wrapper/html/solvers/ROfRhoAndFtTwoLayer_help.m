%% ROfRhoAndFtTwoLayer
% Reflectance as a function of rho and temporal-frequency for a two layer tissue with top layer thickness
%
%% Syntax
%  ROfRhoAndFtTwoLayer(OP, LAYERTHICKNESS, RHO, FT) 
%
%       OP is an N x 2 x 4 matrix of optical properties
%           eg. OP = [[mua11, mus'11, g11, n11] [mua12, mus'12, g12, n12]; ... % layer 1 & layer 2 for system 1
%                 [mua21, mus'21, g21, n21] [mua22, mus'22, g22, n22]; ... % layer 1 & layer 2 for system 2
%                 ];
%       LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%       FT is an 1 x M array of modulation frequencies (in GHz)
%           eg. FT = [0:0.01:0.5];
%
%% Description
% Returns reflectance as a function of source-detector separation (rho =
% sqrt(x*x+y*y)) and temporal-frequency (ft) for a two layer tissue
%
%% Examples
%       layerThickness = 2;  % units: mm
%       opsA = [0.01 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 1
%       opsB = [0.02 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 2
%       opsC = [0.03 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 3
%       op(1,:,:) = [opsA];
%       op(2,:,:) = [opsB];
%       op(3,:,:) = [opsC];
%       rho = 10; % s-d separation, in mm
%       ft = 0:0.01:0.5; % range of temporal frequencies in GHz
%       test = VtsSolvers.ROfRhoAndFtTwoLayer(op, layerThickness, rho, ft);
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