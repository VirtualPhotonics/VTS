%% GetOP
% Summary of function
%
%% Syntax
% GetOP(ABSORBERS, SCATTERER, WAVELENGTHS) gets the optical properties from
% a list of chromophore absorbers and their concentrations for a range of
% wavelengths.
%   
%   ABSORBERS is a class that defines the list of absorbers and their
%   concentrations
%       ABSORBERS.Name is a list of absorber names
%           eg. ABSORBERS.Names = {'HbO2', 'Hb', 'H2O'};
%       ABSORBERS.Concentrations is a list of concentrations
%           eg. ABSORBERS.Concentrations =  [70, 30, 0.8];
%
%   SCATTERER is a class that defines the scatterer
%       SCATTERER.Type (PowerLaw, Intralipid or Mie) Mie is the default
%           eg. SCATTERER.Type = 'PowerLaw';
%               SCATTERER.A = 1.2;
%               SCATTERER.b = 1.42;
%
%   WAVELENGTHS is an 1 x M array of wavelengths
%           eg. WAVELENGTHS = 450:0.5:1000;
%
%% Description
%       Description of function
%
%% Examples
%       Examples go here
%
%       scatterer.Type = 'Intralipid';
%       scatterer.vol_frac = 0.5;
%  
%       scatterer.Type = 'Mie';
%       scatterer.radius = 0.5;
%       scatterer.n = 1.4;
%       scatterer.nMedium = 1.0;
%
%% See Also
% <VtsSpectroscopy_help.html VtsSpectroscopy>
