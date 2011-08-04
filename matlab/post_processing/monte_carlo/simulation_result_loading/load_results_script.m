% script for loading Monte Carlo results

clear all;

% script to parse results from MC simulation
addpath([cd '\xml_toolbox']);

% names of individual MC simulations
datanames = { 'results' };
% datanames = { 'results_mua0.1musp1.0' 'esults_mua0.1musp1.1' }; %...etc

outdir = 'C:\Simulations';

show.RDiffuse =         1;
show.ROfRho =           1;
show.ROfAngle =         1;
show.ROfXAndY =         1;
show.ROfRhoAndTime =    1;
show.ROfRhoAndAngle =   1;
show.ROfRhoAndOmega =   1;
show.TDiffuse =         1;
show.TOfRho =           1;
show.TOfRhoAndAngle =   1;
show.ATotal =           1;
show.AOfRhoAndZ =       1;
show.FluenceOfRhoAndZ = 1;

for mci = 1:length(datanames)
    dataname = datanames{mci};
    results = loadMCResults(outdir, dataname);
    
    if isfield(results, 'RDiffuse') && show.RDiffuse
        disp(['Total reflectance captured by RDiffuse detector: ' num2str(results.RDiffuse.Mean)]);
    end
    
    if isfield(results, 'ROfRho') && show.ROfRho
        figname = 'log(R(\rho))'; figure; plot(results.ROfRho.Rho_Midpoints, log(results.ROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('R(\rho) [mm^-^2]');
        disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(results.ROfRho.Mean(:)))]);
    end
    
    if isfield(results, 'ROfAngle') && show.ROfAngle
        figname = 'log(R(angle))'; figure; plot(results.ROfAngle.Angle_Midpoints, log(results.ROfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('R(angle) [rad^-^1]');
        disp(['Total reflectance captured by ROfAngle detector: ' num2str(sum(results.ROfAngle.Mean(:)))]);
    end
    
    if isfield(results, 'ROfXAndY') && show.ROfXAndY
        figname = 'log(R(x,y))'; figure; imagesc(log(results.ROfXAndY.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfXAndY detector: ' num2str(sum(results.ROfXAndY.Mean(:)))]);
    end
    
    if isfield(results, 'ROfRhoAndTime') && show.ROfRhoAndTime
        figname = 'log(ROfRhoAndTime)'; figure; imagesc(log(results.ROfRhoAndTime.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(sum(results.ROfRhoAndTime.Mean(:)))]);
    end
    
    if isfield(results, 'ROfRhoAndAngle') && show.ROfRhoAndAngle
        figname = 'log(ROfRhoAndAngle)'; figure; imagesc(log(results.ROfRhoAndAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndAngle detector: ' num2str(sum(results.ROfRhoAndAngle.Mean(:)))]);
    end
    
    if isfield(results, 'ROfRhoAndOmega') && show.ROfRhoAndOmega
        figname = 'ROfRhoAndOmega - log(Amplitude)'; figure; imagesc(log(results.ROfRhoAndOmega.Amplitude)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        figname = 'ROfRhoAndOmega - Phase'; figure; imagesc(results.ROfRhoAndOmega.Phase); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndOmega detector: ' num2str(sum(results.ROfRhoAndOmega.Amplitude(:,1)))]);
    end
    
    if isfield(results, 'TDiffuse') && show.TDiffuse
        disp(['Total transmittance captured by TDiffuse detector: ' num2str(results.TDiffuse.Mean)]);
    end
    if isfield(results, 'TOfRho') && show.TOfRho
        figname = 'log(TOfRho)'; figure; imagesc(log(results.TOfRho.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total transmittance captured by TOfRho detector: ' num2str(sum(results.TOfRho.Mean(:)))]);
    end
    if isfield(results, 'TOfAngle') && show.TOfAngle
        figname = 'log(TOfAngle)'; figure; imagesc(log(results.TOfAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total transmittance captured by TOfAngle detector: ' num2str(sum(results.TOfAngle.Mean(:)))]);
    end
    if isfield(results, 'TOfRhoAndAngle') && show.TOfRhoAndAngle
        figname = 'log(TOfRhoAndAngle)'; figure; imagesc(log(results.ROfRhoAndAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total transmittance captured by TOfRhoAndAngle detector: ' num2str(sum(results.TOfRhoAndAngle.Mean(:)))]);
    end
    if isfield(results, 'ATotal') && show.ATotal
        disp(['Total absorption captured by ATotal detector: ' num2str(results.ATotal.Mean)]);
    end
    if isfield(results, 'AOfRhoAndZ') && show.AOfRhoAndZ
        figname = 'log(AOfRhoAndZ)'; figure; imagesc(log(results.AOfRhoAndZ.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Absorbed energy captured by AOfRhoAndZ detector: ' num2str(sum(results.AOfRhoAndZ.Mean(:)))]);
    end
    if isfield(results, 'FluenceOfRhoAndZ') && show.FluenceOfRhoAndZ
        %sum(results.FluenceOfRhoAndZ.Mean(2:end,2:end))
        figname = 'log(FluenceOfRhoAndZ)'; figure; imagesc(log(results.FluenceOfRhoAndZ.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Fluence captured by FluenceOfRhoAndZ detector: ' num2str(sum(results.FluenceOfRhoAndZ.Mean(:)))]);
    end
end