function Chi2=pmc_Chi2(muamus,rhoMidpoints,simInput,measData)
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
% d1 = pMC R(rho)
d1 = DetectorInput.pMCROfRho(rho);
d1.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [mua, musp, gfix, nfix]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
d1.PerturbedRegions = [ 1 ];
ppi.DetectorInputs = { d1 } ;
ppoutput = VtsMonteCarlo.RunPostProcessor(ppi,simInput);
do1 = ppoutput.Detectors(ppoutput.DetectorNames{1});
F = do1.Mean';
Chi2 = (measData - F)*(measData - F)';
end