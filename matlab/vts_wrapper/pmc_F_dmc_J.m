function [F,J]=pmc_F_dmc_J(muamus,rhoMidpoints,simInput,measData)
% the following code assumes 1-layer tissue with varying mua and mus only
% g and n are fixed and not optimized
% specify post-processing of generated database 
ppi = PostProcessorInput();
% determine rho bins from midpoints
rho=zeros(size(rhoMidpoints,2)+1,1);
rho(1) = rhoMidpoints(1) - (rhoMidpoints(2) - rhoMidpoints(1))/2;
rho(2:end) = rhoMidpoints(1:end) + rhoMidpoints(1);
% specify detector based on perturbed optical properties
mua = muamus(1);
musp = muamus(2)*(1-0.8);
gfix = simInput.TissueInput.LayerRegions(2).RegionOP(3);
nfix = simInput.TissueInput.LayerRegions(2).RegionOP(4);
% d1 = pMC R(rho), d2 = dR(rho)/dmua, d3 = dR(rho)/dmus
d1 = DetectorInput.pMCROfRho(rho);
d1.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [mua, musp, gfix, nfix]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
d1.PerturbedRegions = [ 1 ];
d2 = DetectorInput.dMCdROfRhodMua(rho);
d2.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [mua, musp, gfix, nfix]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
d2.PerturbedRegions = [ 1 ];
d3 = DetectorInput.dMCdROfRhodMus(rho);
d3.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [mua, musp, gfix, nfix]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
d3.PerturbedRegions = [ 1 ];
ppi.DetectorInputs = { d1, d2, d3 } ;
ppoutput = VtsMonteCarlo.RunPostProcessor(ppi,simInput);
do1 = ppoutput.Detectors(ppoutput.DetectorNames{1});
do2 = ppoutput.Detectors(ppoutput.DetectorNames{2});
do3 = ppoutput.Detectors(ppoutput.DetectorNames{3});
F = do1.Mean';
% option: normalize forward model by measured data
%F = F./measData;
J = [ do2.Mean do3.Mean ];
J(isnan(J))=0; % set any NaN to 0 so lsqcurvefit doesn't crash
end