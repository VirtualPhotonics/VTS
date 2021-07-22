% Returns mus' based on Steve Jacques' Skin Optics Summary:
% http://omlc.ogi.edu/news/jan98/skinoptics.html

% function to generate mus' = a*lambda^-b + c*lambda^-d
%function [musp]=power_law_scatterer(a,b,c,d,lambda)
%musp=a*lambda**(-b)+c*lambda**(-d);
function [musp,dmusp]=get_musp(scatterers,wavelengths)
musp=scatterers.Coefficients(1)*(wavelengths/1000).^(-scatterers.Coefficients(2));
dmusp=zeros(length(wavelengths),2);
% derivative of lam^-b is (-lam^-b)*ln(lam)
dmusp(:,1)=(wavelengths/1000).^(-scatterers.Coefficients(2));
dmusp(:,2)=-scatterers.Coefficients(1)*(wavelengths/1000).^(-scatterers.Coefficients(2)).*log(wavelengths/1000);
end
