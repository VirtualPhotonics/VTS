% function to read in MCPP infile template, replace strings a1,s1,sp1
% with inverse iterate values and generate pmc and dmc results
function Chi2=pmc_Chi2_ex1(muamus,rhoMidpoints,measData)
% the following code assumes 1-layer tissue with varying mua and mus only
% g and n are fixed and not optimized
% determine rho bins from midpoints
rho=zeros(size(rhoMidpoints,2)+1,1);
rho(1) = rhoMidpoints(1) - (rhoMidpoints(2) - rhoMidpoints(1))/2;
rho(2:end) = rhoMidpoints(1:end) + rhoMidpoints(1);
% specify detector based on perturbed optical properties
% CKH update to read in
gfix = 0.8;
nfix = 1.4;
mua = muamus(1);
if (mua<0) % if mua<0 set to 0
  mua=0.0;
end
mus=muamus(2);
musp = mus*(1-gfix);
% replace MCPP infile with updated OPs
infile_PP='infile_PP_pMC_est.txt';
[status]=system(sprintf('cp infile_PP_pMC_est_template.txt %s',infile_PP));
[status]=system(sprintf('./sub_ops.sh var1 %s %s','rho',infile_PP));
[status]=system(sprintf('./sub_ops.sh a1 %f %s',mua,infile_PP));
[status]=system(sprintf('./sub_ops.sh s1 %f %s',mus,infile_PP));
[status]=system(sprintf('./sub_ops.sh sp1 %f %s',musp,infile_PP));
[status]=system(sprintf('./sub_ops.sh rhostart %f %s',rho(1),infile_PP));
[status]=system(sprintf('./sub_ops.sh rhostop %f %s',rho(end),infile_PP));
[status]=system(sprintf('./sub_ops.sh rhocount %d %s',length(rho),infile_PP));
% run MCPP with updated infile
[status]=system(sprintf('./mc_post infile=%s',infile_PP));
[R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results('PP_rho');
F=pmcR(1:end-1)';
Chi2=(measData-F)*(measData-F)';
end
