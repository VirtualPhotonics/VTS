%% VtsSolvers
%  Definitions for the main solvers in the VTS
%%
% 
% <html>
% For more information about the class see <a href="matlab:doc vtssolvers">VtsSolvers</a>
% </html>
% 
%% FluenceOfRhoAndZ
%   FluenceOfRhoAndZ(OP, RHOS, ZS) 
%   
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   RHO is an 1 x M array of detector locations (in mm)
%       eg. RHO = [1:10];
%   Z is a 1 x M array of z values (in mm)
%       eg. Z = linspace(0.1,19.9,100);
%
%% PHDOfRhoAndZ
%   PHDOfRhoAndZ(OP, RHOS, ZS, SD)
%
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   RHO is an 1 x M array of detector locations (in mm)
%       eg. RHO = [1:10];
%   Z is a 1 x M array of z values (in mm)
%       eg. Z = linspace(0.1,19.9,100);
%   SD is the source-detector separation in
%
%% ROfFx
%   ROfFx(OP, FX) returns the steady-state reflectance in the
%   spatial frequency domain
%
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   FX is an 1 x M array of spatial frequencies (in 1/mm)
%       eg. FX = linspace(0,0.5,11);
%
%% ROfFxAndFt
%   ROfFxAndFt(OP, FX, FT)
%
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   FX is an 1 x M array of spatial frequencies (in 1/mm)
%       eg. FX = linspace(0,0.5,11);
%   FT is an 1 x M array of modulation frequencies (in GHz)
%       eg. FT = [0:0.01:0.5];
%        
%% ROfFxAndT
%   ROfFxAndT(OP, FX, T) 
%
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   FX is an 1 x M array of spatial frequencies (in 1/mm)
%       eg. FX = linspace(0,0.5,11);
%   T is an 1 x O array of times (in ns)
%       eg. T = [1:10];
%
%% ROfRho
%   ROfRho(OP, RHO) returns the steady-state spatially-resolved
%   reflectance 
%   
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   RHO is an 1 x M array of detector locations (in mm)
%       eg. RHO = [1:10];
%
%
%% ROfRhoAndFt
%   ROfRhoAndFt(OP, RHO, FT) 
%
%   OP is an N x 4 matrix of optical properties
%       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
%   RHO is an 1 x M array of detector locations (in mm)
%       eg. RHO = [1:10];
%   FT is an 1 x M array of modulation frequencies (in GHz)
%       eg. FT = [0:0.01:0.5];
%
%% ROfRhoAndT
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