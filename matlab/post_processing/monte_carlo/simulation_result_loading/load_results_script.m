% script for loading Monte Carlo results 

clear all;

% script to parse results from MC simulation
addpath([cd '\xml_toolbox']);

% names of individual MC simulations
datanames = { 'newinfile_results' };
% datanames = { 'newinfile_results_mua0.1musp1.0' 'newinfile_results_mua0.1musp1.1' }; %...etc

outdir = 'C:\Users\dcuccia\Desktop\MonteCarlo_CommandLineApplication';

for mci = 1:length(datanames)    
    dataname = datanames{mci};    
    results = loadMCResults(outdir, dataname);
    
    if isfield(results, 'RDiffuse')
        disp(['Total reflectance captured by RDiffuse detector: ' num2str(results.RDiffuse.Mean)]);
    end

    if isfield(results, 'ROfRho')
        figname = 'log(R(\rho))'; figure; plot(results.ROfRho.Rho_Midpoints, log(results.ROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('R(\rho) [mm^-^2]');
        disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(results.ROfRho.Mean(:)))]);
    end

    if isfield(results, 'ROfAngle')
        figname = 'log(R(angle))'; figure; plot(results.ROfAngle.Angle_Midpoints, log(results.ROfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('R(angle) [rad^-^1]');
        disp(['Total reflectance captured by ROfAngle detector: ' num2str(sum(results.ROfAngle.Mean(:)))]);
    end

    if isfield(results, 'ROfXAndY')
        figname = 'log(R(x,y))'; figure; imagesc(log(results.ROfXAndY.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfXAndY detector: ' num2str(sum(results.ROfXAndY.Mean(:)))]);
    end

    if isfield(results, 'ROfRhoAndTime')
        figname = 'log(ROfRhoAndTime)'; figure; imagesc(log(results.ROfRhoAndTime.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(sum(results.ROfRhoAndTime.Mean(:)))]);
    end

    if isfield(results, 'ROfRhoAndAngle')
        figname = 'log(ROfRhoAndAngle)'; figure; imagesc(log(results.ROfRhoAndAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndAngle detector: ' num2str(sum(results.ROfRhoAndAngle.Mean(:)))]);
    end

    if isfield(results, 'ROfRhoAndOmega')
        figname = 'ROfRhoAndOmega - log(Amplitude)'; figure; imagesc(log(results.ROfRhoAndOmega.Amplitude)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        figname = 'ROfRhoAndOmega - Phase'; figure; imagesc(results.ROfRhoAndOmega.Phase); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndOmega detector: ' num2str(sum(results.ROfRhoAndOmega.Amplitude(:,1)))]);
    end
end