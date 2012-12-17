%% ROfRhoAndT
% Summary of function
%
%% Syntax
%   ROfRhoAndT(OP, RHO, T) returns time-resolved reflectance at
%   specified detector locations
%   
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   RHO is an 1 x M array of detector locations (in mm)
%       eg. RHO = [1:10];
%   T is an 1 x O array of times (in ns)
%       eg. T = [1:10];
%
%% Description
%       Description of function
%
%% Examples
%       Examples go here
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt>