%% ROfFxAndFt
% Reflectance as a function of spatial- and temporal- frequencies
%
%% Syntax
%  ROfFxAndFt(OP, FX, FT)
%
%       OP is an N x 4 matrix of optical properties
%           eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%       FX is an 1 x M array of spatial frequencies (in 1/mm)
%           eg. FX = linspace(0,0.5,11);
%       FT is an 1 x M array of modulation frequencies (in GHz)
%           eg. FT = [0:0.01:0.5];
%
%% Description
% Returns reflectance as a function of spatial-frequencies (fx) and
% temporal-frequencies (ft)
%
%% Examples
%       op = [0.1 1 0.8 1.4]; % optical properties
%       fx = linspace(0,0.5,11); % range of spatial frequencies in 1/mm
%       ft = linspace(0,0.05,51); % range of frequency in GHz
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfFxAndT(op, fx, t);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> | 
% <ROfRhoAndT_help.html ROfRhoAndT> |
% <SetSolverType_help.html SetSolverType>