% function to read in MCPP infile template, replace strings a1,s1,sp1
% with inverse iterate values and generate pmc and dmc results
function [F,J]=pmc_F_dmc_J_ex3(fitparms,wavelengths,rhoMidpoints,absorbers,scatterers,g,n)
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
F=zeros(length(wavelengths)/2,1); % 2=numrho
J=zeros(length(wavelengths)/2,length(fitparms));
% replace MCPP infile with updated OPs
infile_PP='infile_PP_pMC_est.txt';
for iwv=1:length(wavelengths)/2
  [status]=system(sprintf('copy infile_PP_pMC_est_template.txt %s',infile_PP));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %s',infile_PP,'var1',sprintf('wv%d',iwv)));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'a1',ops(iwv,1)));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'s1',ops(iwv,2)/(1-g)));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'sp1',ops(iwv,2)));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'rhostart',rho(1)));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %f',infile_PP,'rhostop',rho(end)));
  [status]=system(sprintf('powershell -inputformat none -file replace_string.ps1 %s %s %d',infile_PP,'rhocount',length(rho)));
  % run MCPP with updated infile
  [status]=system(sprintf('mc_post infile=%s',infile_PP));
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('PP_wv%d',iwv));
  F(iwv)=pmcR(1)'; % first rho=0.1429mm
  F(iwv+length(wavelengths)/2)=pmcR(4)'; % 2nd rho=1mm
  % set jacobian derivative information 
  if (length(fitparms)==length(absorbers.Names)) % => only chromophore fit
    J(iwv,:) = [dmcRmua(1) * dmua(iwv,:)];
    J(iwv+length(wavelengths)/2,:) = [dmcRmua(4) * dmua(iwv,:)];
  elseif (length(fitparms)==length(scatterers.Names)) % => only scatterer fit
    J(iwv,:) = [dmcRmus(1) * dmusp(iwv,:)];
    J(iwv+length(wavelengths)/2,:) = [dmcRmus(4) * dmusp(iwv,:)];
  else
    J(iwv,:) = [dmcRmua(1) * dmua(iwv,:) dmcRmus(4) * dmusp(iwv,:)]; % => both
    J(iwv+length(wavelengths)/2,:) = [dmcRmua(4) * dmua(iwv,:) dmcRmus(7) * dmusp(iwv,:)]; % => both
  end
end
end
