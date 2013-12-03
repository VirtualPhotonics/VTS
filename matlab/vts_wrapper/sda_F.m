function F = sda_F(conc, wv, rho, scatterer)
% the following code assumes that the chromophore concentrations 
% for HbO2, Hb and H2O are being optimized
% scatterer specification is fixed
VtsSolvers.SetSolverType('PointSourceSDA');
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations = [ conc(1), conc(2), conc(3) ];
op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
F = VtsSolvers.ROfRho(op, rho);
end