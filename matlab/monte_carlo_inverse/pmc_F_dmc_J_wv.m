% function to read in MCPP infile template, replace strings a1,s1,sp1
% with inverse iterate values and generate pmc and dmc results
function [F,J]=pmc_F_dmc_J_wv(conc,wavelengths,rhoMidpoints,scatterers,g,n)
% the following code assumes 1-layer tissue with varying mua and mus only
% g and n are fixed and not optimized
% determine rho bins from midpoints
rho=zeros(size(rhoMidpoints,2)+1,1);
rho(1) = rhoMidpoints(1) - (rhoMidpoints(2) - rhoMidpoints(1))/2;
rho(2:end) = rhoMidpoints(1:end) + rhoMidpoints(1);
% specify detector based on perturbed optical properties
absorbers.Names = { 'HbO2','Hb','H2O' };
absorbers.Concentrations = [ conc(1), conc(2), conc(3) ];

[ops,dmua] = get_optical_properties(absorbers, scatterers, wavelengths);
F=zeros(length(wavelengths),1);
J=zeros(length(wavelengths),length(absorbers.Names));
% replace MCPP infile with updated OPs
infile_PP='infile_PP_pMC_est.txt';
for iwv=1:length(wavelengths)
  [status]=system(sprintf('cp infile_PP_pMC_est_template.txt %s',infile_PP));
  [status]=system(sprintf('./sub_ops.sh var1 %s %s',sprintf('wv%d',iwv),infile_PP));
  [status]=system(sprintf('./sub_ops.sh a1 %f %s',ops(iwv,1),infile_PP));
  [status]=system(sprintf('./sub_ops.sh s1 %f %s',ops(iwv,2)/(1-g),infile_PP));
  [status]=system(sprintf('./sub_ops.sh sp1 %f %s',ops(iwv,2),infile_PP));
  [status]=system(sprintf('./sub_ops.sh rhostart %f %s',rho(1),infile_PP));
  [status]=system(sprintf('./sub_ops.sh rhostop %f %s',rho(end),infile_PP));
  [status]=system(sprintf('./sub_ops.sh rhocount %d %s',length(rho),infile_PP))
  % run MCPP with updated infile
  [status]=system(sprintf('./mc_post infile=%s',infile_PP));
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('PP_wv%d',iwv));
  F(iwv)=pmcR(4)';
  % set jacobian derivative information
  J(iwv,:) = dmcRmua(4) * dmua(iwv,:);
end
end
