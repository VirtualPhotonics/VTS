%% GetOP
% Returns the optical properties
%
%% Syntax
% GetOP(ABSORBERS, SCATTERER, WAVELENGTHS)
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
% Returns the optical properties from a list of chromophore absorbers and 
% their concentrations for a range of wavelengths.
%
%% Examples
%       absorbers.Names = {'HbO2', 'Hb', 'H2O'};
%       absorbers.Concentrations =  [70, 30, 0.8];
%       scatterer.Type = 'PowerLaw';
%       scatterer.A = 1.2;
%       scatterer.b = 1.42;
%       wv = 450:0.5:1000;
%       op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
%
%       absorbers.Names = {'HbO2', 'Hb', 'H2O'};
%       absorbers.Concentrations =  [70, 30, 0.8];
%       scatterer.Type = 'Intralipid';
%       scatterer.vol_frac = 0.5;
%       wv = 450:0.5:1000;
%       op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
%  
%       absorbers.Names = {'HbO2', 'Hb', 'H2O'};
%       absorbers.Concentrations =  [70, 30, 0.8];
%       scatterer.Type = 'Mie';
%       scatterer.radius = 0.5;
%       scatterer.n = 1.4;
%       scatterer.nMedium = 1.0;
%       wv = 450:0.5:1000;
%       op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
%
%% See Also
% <VtsSpectroscopy_help.html VtsSpectroscopy>
