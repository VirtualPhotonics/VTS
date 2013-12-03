%% ROfRhoAndFt
% Reflectance as a function of rho and temporal-frequency
%
%% Syntax
%  ROfRhoAndFt(OP, RHO, FT) 
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus1', g1, n1]; [mua2, mus2', g2, n2]; ...];
%           mua and mus' values in (1/mm)
%       RHO is an 1 x M array of detector locations (in mm)
%           eg. RHO = [1:10];
%       FT is an 1 x M array of modulation frequencies (in GHz)
%           eg. FT = [0:0.01:0.5];
%
%% Description
% Returns reflectance as a function of source-detector separation (rho =
% sqrt(x*x+y*y)) and temporal-frequency (ft)
%
%% Examples
%       op = [0.01 1.2 0.8 1.4]; % optical properties
%       rho = 10; % s-d separation, in mm
%       ft = 0:0.01:0.5; % range of temporal frequencies in GHz
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfRhoAndFt(op, rho, ft);
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
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>