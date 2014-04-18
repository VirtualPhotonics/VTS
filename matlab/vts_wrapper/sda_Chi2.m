function Chi2 = sda_Chi2(conc,wv,rho,scatterer,measData)
% the following code assumes that the chromophore concentrations 
% for HbO2, Hb and H2O are being optimized
% scatterer specification is fixed
% wv = parameters{1};
% rho = parameters{2};
% scatterer = parameters{3};
% measData = parameters{4};
VtsSolvers.SetSolverType('PointSourceSDA');
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations = [ conc(1), conc(2), conc(3) ];
op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
F = VtsSolvers.ROfRho(op, rho);
Chi2 = (measData - F)*(measData - F)';
end