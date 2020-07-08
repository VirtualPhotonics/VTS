% function to read in MCPP infile template, replace strings a1,s1,sp1
% with inverse iterate values and generate pmc and dmc results
function F=pmc_F_dmc_J_wv(conc,wavelengths,rhoMidpoints,scatterers)
% the following code assumes 1-layer tissue with varying mua and mus only
% g and n are fixed and not optimized
% determine rho bins from midpoints
if (length(rhoMidpoints)>1)
  rho=zeros(size(rhoMidpoints,2)+1,1);
  rho(1) = rhoMidpoints(1) - (rhoMidpoints(2) - rhoMidpoints(1))/2;
  rho(2:end) = rhoMidpoints(1:end) + rhoMidpoints(1);
else
  rho(1)=rhoMidpoints(1)-0.5;  % FIX! not so great solution
  rho(2)=rhoMidpoints(1)+0.5;
end
% specify detector based on perturbed optical properties
absorbers.Names = { 'HbO2','Hb','HbO' };
absorbers.Concentrations = [ conc(1), conc(2), conc(3) ];

ops = get_optical_properties(absorbers, scatterers, wavelengths);
% CKH update to read in
gfix = 0.8;
nfix = 1.4;
F=zeros(length(wavelengths),1);
% replace MCPP infile with updated OPs
infile_PP='infile_PP_pMC_est.txt';
for iwv=1:length(wavelengths)
  [status]=system(sprintf('cp infile_PP_pMC_est_template.txt %s',infile_PP));
  [status]=system(sprintf('./sub_ops.sh var1 %s %s',sprintf('wv%d',iwv),infile_PP));
  [status]=system(sprintf('./sub_ops.sh a1 %f %s',ops(iwv,1),infile_PP));
  [status]=system(sprintf('./sub_ops.sh s1 %f %s',ops(iwv,2)/(1-gfix),infile_PP));
  [status]=system(sprintf('./sub_ops.sh sp1 %f %s',ops(iwv,2),infile_PP));
  [status]=system(sprintf('./sub_ops.sh rhostart %f %s',rho(1),infile_PP));
  [status]=system(sprintf('./sub_ops.sh rhostop %f %s',rho(end),infile_PP));
  [status]=system(sprintf('./sub_ops.sh rhocount %d %s',length(rho),infile_PP))
  % run MCPP with updated infile
  [status]=system(sprintf('./mc_post infile=%s',infile_PP));
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('PP_wv%d',iwv));
  F(iwv)=pmcR(1)';
end
end
