% script for loading Monte Carlo results

clear all;
dbstop if error;
slash = filesep;  % get correct path delimiter for platform

% script to parse results from MC simulation
addpath([cd slash 'xml_toolbox']);

% names of individual MC simulations
% datanames = { 'one_layer_all_detectors' };
datanames = { 'six_layer_ReflectedTimeOfRhoAndSubregionHist' };
% datanames = { 'results_mua0.1musp1.0' 'esults_mua0.1musp1.1' }; %...etc

%outdir = 'C:\Simulations';
outdir = '.';

show.RDiffuse =                 1;
show.ROfRho =                   1;
show.ROfAngle =                 1;
show.ROfXAndY =                 1;
show.ROfRhoAndTime =            1;
show.ROfRhoAndAngle =           1;
show.ROfRhoAndOmega =           1;
show.TDiffuse =                 1;
show.TOfRho =                   1;
show.TOfRhoAndAngle =           1;
show.TOfAngle =                 1;
show.ATotal =                   1;
show.AOfRhoAndZ =               1;
show.FluenceOfRhoAndZ =         1;
show.RadianceOfRhoAndZAndAngle = 1;
show.pMCROfRho =                1;
show.pMCROfRhoAndTime =         1;
show.ReflectedMTOfRhoAndSubregionHist = 1;
show.ReflectedTimeOfRhoAndSubregionHist = 1;

for mci = 1:length(datanames)
  dataname = datanames{mci};
  results = loadMCResults(outdir, dataname);  
  for di = 1:size(results, 2)
    if isfield(results{di}, 'RDiffuse') && show.RDiffuse
        disp(['Total reflectance captured by RDiffuse detector: ' num2str(results{di}.RDiffuse.Mean)]);
    end

    if isfield(results{di}, 'ROfRho') && show.ROfRho
        figname = sprintf('log(%s)',results{di}.ROfRho.Name); figure; plot(results{di}.ROfRho.Rho_Midpoints, log10(results{di}.ROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('R(\rho) [mm^-^2]');
        disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(results{di}.ROfRho.Mean(:)))]);
    end

    if isfield(results{di}, 'ROfAngle') && show.ROfAngle
        figname = sprintf('log(%s)',results{di}.ROfAngle.Name); figure; plot(results{di}.ROfAngle.Angle_Midpoints, log(results{di}.ROfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('R(angle) [rad^-^1]');
        disp(['Total reflectance captured by ROfAngle detector: ' num2str(sum(results{di}.ROfAngle.Mean(:)))]);
    end

    if isfield(results{di}, 'ROfXAndY') && show.ROfXAndY
        figname = sprintf('log(%s)',results{di}.ROfXAndY.Name); figure; imagesc(log(results{di}.ROfXAndY.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfXAndY detector: ' num2str(sum(results{di}.ROfXAndY.Mean(:)))]);
    end

    if isfield(results{di}, 'ROfRhoAndTime') && show.ROfRhoAndTime
        figname = sprintf('log(%s)',results{di}.ROfRhoAndTime.Name); figure; imagesc(log(results{di}.ROfRhoAndTime.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(sum(results{di}.ROfRhoAndTime.Mean(:)))]);
    end

    if isfield(results{di}, 'ROfRhoAndAngle') && show.ROfRhoAndAngle
        figname = sprintf('log(%s)',results{di}.ROfRhoAndAngle.Name); figure; imagesc(log(results{di}.ROfRhoAndAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndAngle detector: ' num2str(sum(results{di}.ROfRhoAndAngle.Mean(:)))]);
    end

    if isfield(results{di}, 'ROfRhoAndOmega') && show.ROfRhoAndOmega
        figname = sprintf('%s - log(Amplitude)',results{di}.ROfRhoAndOmega.Name); figure; imagesc(log(results{di}.ROfRhoAndOmega.Amplitude)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        figname = sprintf('%s - Phase',results{di}.ROfRhoAndOmega.Name); figure; imagesc(results{di}.ROfRhoAndOmega.Phase); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by ROfRhoAndOmega detector: ' num2str(sum(results{di}.ROfRhoAndOmega.Amplitude(:,1)))]);
    end

    if isfield(results{di}, 'TDiffuse') && show.TDiffuse
        disp(['Total transmittance captured by TDiffuse detector: ' num2str(results{di}.TDiffuse.Mean)]);
    end
    if isfield(results{di}, 'TOfRho') && show.TOfRho
         figname = sprintf('log(%s)',results{di}.TOfRho.Name); figure; plot(results{di}.TOfRho.Rho_Midpoints, log10(results{di}.TOfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('T(\rho) [mm^-^2]');
         disp(['Total transmittance captured by TOfRho detector: ' num2str(sum(results{di}.TOfRho.Mean(:)))]);
    end
    if isfield(results{di}, 'TOfAngle') && show.TOfAngle
        figname = sprintf('log(%s)',results{di}.TOfAngle.Name); figure; plot(results{di}.TOfAngle.Angle_Midpoints, log(results{di}.TOfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('T(angle) [rad^-^1]');
        disp(['Total transmittance captured by TOfAngle detector: ' num2str(sum(results{di}.TOfAngle.Mean(:)))]);
    end
    if isfield(results{di}, 'TOfRhoAndAngle') && show.TOfRhoAndAngle
        figname = sprintf('log(%s)',results{di}.TOfRhoAndAngle.Name); figure; imagesc(log(results{di}.TOfRhoAndAngle.Mean)); axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total transmittance captured by TOfRhoAndAngle detector: ' num2str(sum(results{di}.TOfRhoAndAngle.Mean(:)))]);
    end
    if isfield(results{di}, 'ATotal') && show.ATotal
        disp(['Total absorption captured by ATotal detector: ' num2str(results{di}.ATotal.Mean)]);
    end
    if isfield(results{di}, 'AOfRhoAndZ') && show.AOfRhoAndZ
        figname = sprintf('log(%s)',results{di}.AOfRhoAndZ.Name); figure; imagesc(log(results{di}.AOfRhoAndZ.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Absorbed energy captured by AOfRhoAndZ detector: ' num2str(sum(results{di}.AOfRhoAndZ.Mean(:)))]);
    end
    if isfield(results{di}, 'FluenceOfRhoAndZ') && show.FluenceOfRhoAndZ
        %sum(results{di}.FluenceOfRhoAndZ.Mean(2:end,2:end))
        figname = sprintf('log(%s)',results{di}.FluenceOfRhoAndZ.Name); figure; imagesc(log(results{di}.FluenceOfRhoAndZ.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Fluence captured by FluenceOfRhoAndZ detector: ' num2str(sum(results{di}.FluenceOfRhoAndZ.Mean(:)))]);
    end
    if isfield(results{di}, 'RadianceOfRhoAndZAndAngle') && show.RadianceOfRhoAndZAndAngle
        %sum(results{di}.RadianceOfRhoAndZAndAngle.Mean(2:end,2:end,2:end))
        numangles = length(results{di}.RadianceOfRhoAndZAndAngle.Angle) - 1;
        for i=1:numangles
            figname = sprintf('log(%s) %5.3f<angle<%5.3f',results{di}.RadianceOfRhoAndZAndAngle.Name,(i-1)*pi/numangles,i*pi/numangles); figure; imagesc(log(results{di}.RadianceOfRhoAndZAndAngle.Mean(:,:,i))); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        end
        disp(['Radiance captured by RadianceOfRhoAndZ detector: ' num2str(sum(results{di}.RadianceOfRhoAndZAndAngle.Mean(:)))]);
    end
    if isfield(results{di}, 'ReflectedMTOfRhoAndSubregionHist') && show.ReflectedMTOfRhoAndSubRegionHist
        numtissueregions = length(results{di}.ReflectedMTOfRhoAndSubregionHist.SubregionIndices);
        for i=1:numtissueregions
            figname = sprintf('log(%s) Region Index %d',results{di}.ReflectedMTOfRhoAndSubregionHist.Name, i-1); figure; imagesc(log(reshape(results{di}.ReflectedMTOfRhoAndSubregionHist.Mean(:,i,:),...
               length(results{di}.ReflectedMTOfRhoAndSubregionHist.Rho)-1,length(results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins)-1)'));...        
               colorbar; title(figname); set(gcf,'Name', figname);
        end
        disp(['Momentum Transfer captured by ReflectedMTOfRhoAndSubregionHist detector: ' num2str(sum(results{di}.ReflectedMTOfRhoAndSubregionHist.Mean(:)))]);
    end
    if isfield(results{di}, 'ReflectedTimeOfRhoAndSubregionHist') && show.ReflectedTimeOfRhoAndSubregionHist
        numtissueregions = length(results{di}.ReflectedTimeOfRhoAndSubregionHist.SubregionIndices);
        for i=1:numtissueregions
            figname = sprintf('%s Region Index %d',results{di}.ReflectedTimeOfRhoAndSubregionHist.Name, i-1); figure; imagesc((reshape(results{di}.ReflectedTimeOfRhoAndSubregionHist.Mean(:,i,:),...
               length(results{di}.ReflectedTimeOfRhoAndSubregionHist.Rho)-1,length(results{di}.ReflectedTimeOfRhoAndSubregionHist.Time)-1)'));...        
               colorbar; title(figname); set(gcf,'Name', figname);
        end
        disp(['Time in Subregion captured by ReflectedTimeOfRhoAndSubregionHist detector: ' num2str(sum(results{di}.ReflectedTimeOfRhoAndSubregionHist.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCROfRho') && show.pMCROfRho
        figname = sprintf('log(%s)',results{di}.pMCROfRho.Name); figure; plot(results{di}.pMCROfRho.Rho_Midpoints, log10(results{di}.pMCROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('pMC R(\rho) [mm^-^2]');
        disp(['Total reflectance captured by pMCROfRho detector: ' num2str(sum(results{di}.pMCROfRho.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCROfRhoAndTime') && show.pMCROfRhoAndTime
        figname = sprintf('log(%s)',results{di}.pMCROfRhoAndTime.Name); figure; imagesc(log(results{di}.pMCROfRhoAndTime.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
        disp(['Total reflectance captured by pMCROfRhoAndTime detector: ' num2str(sum(results{di}.pMCROfRhoAndTime.Mean(:)))]);
    end
  end
end