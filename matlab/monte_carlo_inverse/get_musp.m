% Returns mus' based on Steve Jacques' Skin Optics Summary:
% http://omlc.ogi.edu/news/jan98/skinoptics.html

% function to generate mus' = a*lambda^-b + c*lambda^-d
%function [musp]=power_law_scatterer(a,b,c,d,lambda)
%musp=a*lambda**(-b)+c*lambda**(-d);
function [musp]=get_musp(scatterers,wavelengths)
musp=scatterers.a*(wavelengths/1000).^(-scatterers.b);
end
