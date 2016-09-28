%% ROfRhoAndTimeTwoLayer
% Reflectance as a function of rho and time for a two layer tissue with specified top
% layer thickness
%
%% Syntax
%  ROfRhoAndTimeTwoLayer(OP, THICKNESS, RHO, T) 
%   
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       THICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%       T is an 1 x O array of times (in ns)
%           eg. T = [1:10];
%
%% Description
% Returns reflectance as a function of source-detector separation
% (rho = sqrt(x*x+y*y)) and time (t) for a two layer tissue
%
%% Examples
%       wv = 650:100:850;
%       % create a list of chromophore absorbers and their concentrations
%       absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
%       absorbers.Concentrations =  [70,     30,   0.8  ];
%       % create a scatterer (PowerLaw, Intralipid, or Mie)
%       scatterer.Type = 'PowerLaw';
%       scatterer.A = 1.2;
%       scatterer.b = 1.42;
%       opBottomLayer = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
%       % get OPs at first wavelength and perturb top layer mua by factor 1.1
%       opsA = [opBottomLayer(1,1) opBottomLayer(1,2) opBottomLayer(1,3) opBottomLayer(1,4);
%             1.1*opBottomLayer(1,1) opBottomLayer(1,2) opBottomLayer(1,3) opBottomLayer(1,4)];
%       % get OPs at first wavelength and perturb top layer mua
%       opsB = [opBottomLayer(2,1) opBottomLayer(2,2) opBottomLayer(2,3) opBottomLayer(2,4);
%             1.1*opBottomLayer(2,1) opBottomLayer(2,2) opBottomLayer(2,3) opBottomLayer(2,4)];
%       % get OPs at first wavelength and perturb top layer mua
%       opsC = [opBottomLayer(3,1) opBottomLayer(3,2) opBottomLayer(3,3) opBottomLayer(3,4);
%             1.1*opBottomLayer(3,1) opBottomLayer(3,2) opBottomLayer(3,3) opBottomLayer(3,4)];
%       op(1,:,:) = [opsA];
%       op(2,:,:) = [opsB];
%       op(3,:,:) = [opsC];
%       rho = 10; %s-d separation, in mm
%       t = 0:0.001:0.5; % range of times in ns
%       reflectance = VtsSolvers.ROfRhoAndTimeTwoLayer(op, layerThickness, rho, t);
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