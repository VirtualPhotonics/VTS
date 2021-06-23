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
mus=muamus(2);
musp = mus*(1-gfix);
% replace MCPP infile with updated OPs
infile_PP='infile_PP_pMC_est.txt';
[status]=system(sprintf('copy infile_PP_pMC_est_template.txt %s',infile_PP));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %s',infile_PP,'var1','rho'));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'a1',mua));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'s1',mus));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'sp1',musp));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'rhostart',rho(1)));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'rhostop',rho(end)));
[status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %d',infile_PP,'rhocount',length(rho)));
% run MCPP with updated infile
[status]=system(sprintf('mc_post infile=%s',infile_PP));
[R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results('PP_rho');
F=pmcR(1:end-1)';
Chi2=(measData-F)*(measData-F)';
end
