%% ROfFxTwoLayer
% Spatial-frequency reflectance for a two layer tissue with specified top layer
% thickness
%
%% Syntax
%  ROfFxTwoLayer(OP, LAYERTHICKNESS, FX) 
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
%       FX is an 1 x M array of spatial frequencies (in 1/mm)
%           eg. FX = linspace(0,0.5,11);
%
%% Description
% Returns steady-state reflectance in the spatial frequency domain for a
% two layer tissue with specified top layer thickness
%
%% Examples
%       layerThickness = 2;  % units: mm
%       opsA = [0.01 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 1
%       opsB = [0.02 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 2
%       opsC = [0.03 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 3
%       op(1,:,:) = [opsA];
%       op(2,:,:) = [opsB];
%       op(3,:,:) = [opsC];
%       fx = 0:0.01:0.5; % range of spatial frequencies in 1/mm
%       reflectance = VtsSolvers.ROfFxTwoLayer(op, layerThickness, fx);
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