% script for loading forward R(rho) and inverse pMC/dMC R(rho) results
function [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(dataname)

slash = filesep;  % get correct path delimiter for platform
% script to parse results from MC simulation
% addpath([cd slash 'xml_toolbox']);
addpath([pwd slash 'jsonlab']);

% names of individual MC simulations
datanames = { dataname };

% outdir = 'C:\Projects\vts\src\Vts.MonteCarlo.CommandLineApplication\bin\Release';
outdir = '.';

R=0;
pmcR=0;
dmcRmua=0;
dmcRmus=0;

show.ROfRho =            	1;
show.pMCROfRho =		1; 
show.dMCdROfRhodMua = 		1;
show.dMCdROfRhodMus = 		1;
for mci = 1:length(datanames)
  dataname = datanames{mci};
  results = loadMCResults(outdir, dataname);  
  for di = 1:size(results, 2)
    if isfield(results{di}, 'ROfRho') && show.ROfRho
        R=results{di}.ROfRho.Mean;
        disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(results{di}.ROfRho.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCROfRho') && show.pMCROfRho
        pmcR=results{di}.pMCROfRho.Mean;
        disp(['Total reflectance captured by pMCROfRho detector: ' num2str(sum(results{di}.pMCROfRho.Mean(:)))]);
    end
    if isfield(results{di}, 'dMCdROfRhodMua') && show.dMCdROfRhodMua
        dmcRmua=results{di}.dMCdROfRhodMua.Mean;
    end
    if isfield(results{di}, 'dMCdROfRhodMus') && show.dMCdROfRhodMus
        dmcRmus=results{di}.dMCdROfRhodMus.Mean;
    end
  end
end
