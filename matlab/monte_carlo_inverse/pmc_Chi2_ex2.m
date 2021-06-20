% function to read in MCPP infile template, replace strings a1,s1,sp1
% with inverse iterate values and generate pmc and dmc results
function Chi2=pmc_Chi2_ex2(fitparms,wavelengths,rhoMidpoints,absorbers,scatterers,g,n,measData)
% the following code assumes 1-layer tissue with varying mua and mus only
% g and n are fixed and not optimized
% determine rho bins from midpoints
rho=zeros(size(rhoMidpoints,2)+1,1);
rho(1) = rhoMidpoints(1) - (rhoMidpoints(2) - rhoMidpoints(1))/2;
rho(2:end) = rhoMidpoints(1:end) + rhoMidpoints(1);
% determine if fitting chromophore concentrations 
i=0;
if (length(absorbers.Names)>0)
  absorbers.Concentrations=zeros(1,length(absorbers.Names));
  for i=1:length(absorbers.Names)
    absorbers.Concentrations(1,i)=fitparms(i);
  end
end
% and/or scattering coefficients
if (length(scatterers.Names)>0)
  scatterers.Coefficients=zeros(1,length(scatterers.Names));
  for j=1:length(scatterers.Names)
    scatterers.Coefficients(1,j)=fitparms(j+i);
  end
end
[ops,dmua,dmusp] = get_optical_properties(absorbers, scatterers, wavelengths);
F=zeros(1,length(wavelengths));
% replace MCPP infile with updated OPs
infile_PP='infile_PP_pMC_est.txt';
for iwv=1:length(wavelengths)
  if (ops(iwv,1)<0) % if mua<0 set to 0
    ops(iwv,1)=0;
  end
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
  F(iwv)=pmcR(4);
end
Chi2=(measData-F)*(measData-F)';
end
