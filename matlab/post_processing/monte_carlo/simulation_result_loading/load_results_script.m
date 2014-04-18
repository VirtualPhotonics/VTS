% script for loading Monte Carlo results

clear all;
dbstop if error;
slash = filesep;  % get correct path delimiter for platform

% script to parse results from MC simulation
addpath([cd slash 'xml_toolbox']);

% names of individual MC simulations
%datanames = { 'three_layer_normal_source_ROfRho_FluenceOfXAndYAndZ' };
%datanames = { 'ellip_450nm_g0p9' };
datanames = { 'one_layer_all_detectors' };
% datanames = { 'results_mua0.1musp1.0' 'esults_mua0.1musp1.1' }; %...etc

% outdir = 'C:\Projects\vts\src\Vts.MonteCarlo.CommandLineApplication\bin\Release';
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
show.FluenceOfXAndYAndZ =       1;
show.FluenceOfRhoAndZAndTime =  1;
show.RadianceOfRhoAndZAndAngle = 1;
show.RadianceOfXAndYAndZAndThetaAndPhi = 1;
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
        rhodelta = results{di}.ROfRho.Rho(2)-results{di}.ROfRho.Rho(1);
        rhonorm = 2 * pi * (results{di}.ROfRho.Rho_Midpoints * rhodelta);
        disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(results{di}.ROfRho.Mean.*rhonorm'))]);
    end

    if isfield(results{di}, 'ROfAngle') && show.ROfAngle
        figname = sprintf('log(%s)',results{di}.ROfAngle.Name); figure; plot(results{di}.ROfAngle.Angle_Midpoints, log(results{di}.ROfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('R(angle) [rad^-^1]');
        angledelta = results{di}.ROfAngle.Angle(2)-results{di}.ROfAngle.Angle(1);
        anglenorm = 2 * pi * sin(results{di}.ROfAngle.Angle_Midpoints * angledelta) * angledelta;
        disp(['Total reflectance captured by ROfAngle detector: ' num2str(sum(results{di}.ROfAngle.Mean.*anglenorm'))]);
    end

    if isfield(results{di}, 'ROfXAndY') && show.ROfXAndY
        figname = sprintf('log(%s)',results{di}.ROfXAndY.Name); figure; imagesc(results{di}.ROfXAndY.Y_Midpoints, results{di}.ROfXAndY.X_Midpoints, log(results{di}.ROfXAndY.Mean')); colorbar; title(figname); set(gcf,'Name', figname); ylabel('Y [mm]'); xlabel('X [mm]');
        xynorm = (results{di}.ROfXAndY.X(2)-results{di}.ROfXAndY.X(1))*(results{di}.ROfXAndY.Y(2)-results{di}.ROfXAndY.Y(1));
        disp(['Total reflectance captured by ROfXAndY detector: ' num2str(sum(results{di}.ROfXAndY.Mean(:)*xynorm))]);
    end

    if isfield(results{di}, 'ROfRhoAndTime') && show.ROfRhoAndTime
        numtimes = length(results{di}.ROfRhoAndTime.Time)-1;
        figname = sprintf('log(%s)',results{di}.ROfRhoAndTime.Name); figure; imagesc(results{di}.ROfRhoAndTime.Rho_Midpoints, results{di}.ROfRhoAndTime.Time_Midpoints,log(results{di}.ROfRhoAndTime.Mean')); colorbar; title(figname); set(gcf,'Name', figname); ylabel('time [ns]'); xlabel('\rho [mm]');
        rhodelta = results{di}.ROfRhoAndTime.Rho(2)-results{di}.ROfRhoAndTime.Rho(1);
        timedelta = results{di}.ROfRhoAndTime.Time(2)-results{di}.ROfRhoAndTime.Time(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndTime.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(sum(sum(timedelta*results{di}.ROfRhoAndTime.Mean.*repmat(rhonorm',1,numtimes))))]);
    end

    if isfield(results{di}, 'ROfRhoAndAngle') && show.ROfRhoAndAngle        
        figname = sprintf('log(%s)',results{di}.ROfRhoAndAngle.Name); figure; imagesc(results{di}.ROfRhoAndAngle.Rho_Midpoints, results{di}.ROfRhoAndAngle.Angle_Midpoints, log(results{di}.ROfRhoAndAngle.Mean')); colorbar; title(figname); set(gcf,'Name', figname);ylabel('\angle [rad]'); xlabel('\rho [mm]'); 
        rhodelta = results{di}.ROfRhoAndAngle.Rho(2)-results{di}.ROfRhoAndAngle.Rho(1);
        angledelta = results{di}.ROfRhoAndAngle.Angle(2)-results{di}.ROfRhoAndAngle.Angle(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndAngle.Rho_Midpoints * rhodelta;
        anglenorm = 2 * pi * sin(results{di}.ROfRhoAndAngle.Angle_Midpoints * angledelta) * angledelta;
        disp(['Total reflectance captured by ROfRhoAndAngle detector: ' num2str(sum(sum(results{di}.ROfRhoAndAngle.Mean.*repmat(rhonorm',[1,size(anglenorm,2)]).*repmat(anglenorm,[size(rhonorm,2),1]))))]);
    end

    if isfield(results{di}, 'ROfRhoAndOmega') && show.ROfRhoAndOmega
        figname = sprintf('%s - log(Amplitude)',results{di}.ROfRhoAndOmega.Name); figure; imagesc(results{di}.ROfRhoAndOmega.Rho_Midpoints, results{di}.ROfRhoAndOmega.Omega_Midpoints, log(results{di}.ROfRhoAndOmega.Amplitude')); colorbar; title(figname); set(gcf,'Name', figname); ylabel('\omega [GHz]'); xlabel('\rho [mm]');
        figname = sprintf('%s - Phase',results{di}.ROfRhoAndOmega.Name); figure; imagesc(results{di}.ROfRhoAndOmega.Rho_Midpoints, results{di}.ROfRhoAndOmega.Omega_Midpoints, results{di}.ROfRhoAndOmega.Phase'); colorbar; title(figname); set(gcf,'Name', figname); ylabel('\omega [GHz]'); xlabel('\rho [mm]');
        rhodelta = results{di}.ROfRhoAndOmega.Rho(2)-results{di}.ROfRhoAndOmega.Rho(1);
        rhonorm = 2 * pi * (results{di}.ROfRhoAndOmega.Rho_Midpoints * rhodelta);
        disp(['Total reflectance captured by ROfRhoAndOmega detector: ' num2str(sum(results{di}.ROfRhoAndOmega.Amplitude(:,1).*rhonorm'))]);
    end

    if isfield(results{di}, 'TDiffuse') && show.TDiffuse
        disp(['Total transmittance captured by TDiffuse detector: ' num2str(results{di}.TDiffuse.Mean)]);
    end
    if isfield(results{di}, 'TOfRho') && show.TOfRho
         figname = sprintf('log(%s)',results{di}.TOfRho.Name); figure; plot(results{di}.TOfRho.Rho_Midpoints, log10(results{di}.TOfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('T(\rho) [mm^-^2]');
         rhodelta = results{di}.TOfRho.Rho(2)-results{di}.TOfRho.Rho(1);
         rhonorm = 2 * pi * (results{di}.TOfRho.Rho_Midpoints * rhodelta);
         disp(['Total reflectance captured by TOfRho detector: ' num2str(sum(results{di}.TOfRho.Mean.*rhonorm'))]);
    end
    if isfield(results{di}, 'TOfAngle') && show.TOfAngle
        figname = sprintf('log(%s)',results{di}.TOfAngle.Name); figure; plot(results{di}.TOfAngle.Angle_Midpoints, log(results{di}.TOfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('T(angle) [rad^-^1]');
        angledelta = results{di}.TOfAngle.Angle(2)-results{di}.TOfAngle.Angle(1);
        anglenorm = 2 * pi * sin(results{di}.TOfAngle.Angle_Midpoints * angledelta) * angledelta;
        disp(['Total reflectance captured by TOfAngle detector: ' num2str(sum(results{di}.TOfAngle.Mean.*anglenorm'))]);
    end
    if isfield(results{di}, 'TOfRhoAndAngle') && show.TOfRhoAndAngle
        figname = sprintf('log(%s)',results{di}.TOfRhoAndAngle.Name); figure; imagesc(results{di}.TOfRhoAndAngle.Rho_Midpoints, results{di}.TOfRhoAndAngle.Angle_Midpoints, log(results{di}.TOfRhoAndAngle.Mean')); colorbar; title(figname); set(gcf,'Name', figname);ylabel('\angle [rad]');xlabel('\rho [mm]');
        rhodelta = results{di}.TOfRhoAndAngle.Rho(2)-results{di}.TOfRhoAndAngle.Rho(1);
        angledelta = results{di}.TOfRhoAndAngle.Angle(2)-results{di}.TOfRhoAndAngle.Angle(1);
        rhonorm = 2 * pi * results{di}.TOfRhoAndAngle.Rho_Midpoints * rhodelta;
        anglenorm = 2 * pi * sin(results{di}.TOfRhoAndAngle.Angle_Midpoints * angledelta) * angledelta;
        disp(['Total transmittance captured by TOfRhoAndAngle detector: ' num2str(sum(sum(results{di}.TOfRhoAndAngle.Mean.*repmat(rhonorm',[1,size(anglenorm,2)]).*repmat(anglenorm,[size(rhonorm,2),1]))))]);
    end
    if isfield(results{di}, 'ATotal') && show.ATotal
        disp(['Total absorption captured by ATotal detector: ' num2str(results{di}.ATotal.Mean)]);
    end
    if isfield(results{di}, 'AOfRhoAndZ') && show.AOfRhoAndZ
        numzs = length(results{di}.AOfRhoAndZ.Z)-1;
        figname = sprintf('log(%s)',results{di}.AOfRhoAndZ.Name); figure; imagesc(results{di}.AOfRhoAndZ.Rho_Midpoints, results{di}.AOfRhoAndZ.Z_Midpoints, log(results{di}.AOfRhoAndZ.Mean')); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]');xlabel('\rho mm]');
        rhodelta = results{di}.AOfRhoAndZ.Rho(2)-results{di}.AOfRhoAndZ.Rho(1);
        zdelta = results{di}.AOfRhoAndZ.Z(2)-results{di}.AOfRhoAndZ.Z(1);
        rhonorm = 2 * pi * results{di}.AOfRhoAndZ.Rho_Midpoints * rhodelta;
        disp(['Absorbed energy captured by AOfRhoAndZ detector: ' num2str(sum(sum(zdelta*results{di}.AOfRhoAndZ.Mean.*repmat(rhonorm',[1,numzs]))))]);
    end
    if isfield(results{di}, 'FluenceOfRhoAndZ') && show.FluenceOfRhoAndZ
        numzs = length(results{di}.FluenceOfRhoAndZ.Z)-1;
        figname = sprintf('log(%s)',results{di}.FluenceOfRhoAndZ.Name); figure; imagesc(results{di}.FluenceOfRhoAndZ.Rho_Midpoints, results{di}.FluenceOfRhoAndZ.Z_Midpoints, log(results{di}.FluenceOfRhoAndZ.Mean')); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]');
        rhodelta = results{di}.FluenceOfRhoAndZ.Rho(2)-results{di}.FluenceOfRhoAndZ.Rho(1);
        zdelta = results{di}.FluenceOfRhoAndZ.Z(2)-results{di}.FluenceOfRhoAndZ.Z(1);
        normrho = 2 * pi * results{di}.FluenceOfRhoAndZ.Rho_Midpoints * rhodelta;
        disp(['Fluence captured by FluenceOfRhoAndZ detector: ' num2str(sum(sum(zdelta*results{di}.FluenceOfRhoAndZ.Mean.*repmat(normrho',[1,numzs]))))]);
    end
    if isfield(results{di}, 'FluenceOfRhoAndZAndTime') && show.FluenceOfRhoAndZAndTime
        numtimes = length(results{di}.FluenceOfRhoAndZAndTime.Time)-1;
        numzs = length(results{di}.FluenceOfRhoAndZAndTime.Z)-1;
        for i=1:10:numtimes % do every 10 time bins
            figname = sprintf('log(%s) time=%5.3f ns',results{di}.FluenceOfRhoAndZAndTime.Name,results{di}.FluenceOfRhoAndZAndTime.Time_Midpoints(i)); figure; imagesc(results{di}.FluenceOfRhoAndZAndTime.Rho_Midpoints, results{di}.FluenceOfRhoAndZAndTime.Z_Midpoints, log(squeeze(results{di}.FluenceOfRhoAndZAndTime.Mean(:,:,i))')); 
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]'); 
        end
        rhodelta = results{di}.FluenceOfRhoAndZAndTime.Rho(2)-results{di}.FluenceOfRhoAndZAndTime.Rho(1);
        timedelta = results{di}.FluenceOfRhoAndZAndTime.Time(2)-results{di}.FluenceOfRhoAndZAndTime.Time(1);
        zdelta = results{di}.FluenceOfRhoAndZAndTime.Z(2)-results{di}.FluenceOfRhoAndZAndTime.Z(1);
        rhonorm = 2 * pi * results{di}.FluenceOfRhoAndZAndTime.Rho_Midpoints * rhodelta;
        disp(['Fluence captured by FluenceOfRhoAndZAndTime detector: ' num2str(sum(sum(sum(timedelta*zdelta*results{di}.FluenceOfRhoAndZAndTime.Mean.*repmat(rhonorm',[1,numzs,numtimes])))))]);
    end
    if isfield(results{di}, 'FluenceOfXAndYAndZ') && show.FluenceOfXAndYAndZ
        numY = length(results{di}.FluenceOfXAndYAndZ.Y) - 1;
        for i=1:numY
            figname = sprintf('log(%s) y=%5.3f',results{di}.FluenceOfXAndYAndZ.Name,results{di}.FluenceOfXAndYAndZ.Y_Midpoints(i)); figure; imagesc(results{di}.FluenceOfXAndYAndZ.X_Midpoints, results{di}.FluenceOfXAndYAndZ.Z_Midpoints, log(squeeze(results{di}.FluenceOfXAndYAndZ.Mean(:,i,:))')); 
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]');
            xyznorm = (results{di}.FluenceOfXAndYAndZ.X(2)-results{di}.FluenceOfXAndYAndZ.X(1))*(results{di}.FluenceOfXAndYAndZ.Y(2)-results{di}.FluenceOfXAndYAndZ.Y(1))*(results{di}.FluenceOfXAndYAndZ.Z(2)-results{di}.FluenceOfXAndYAndZ.Z(1));
            disp(['Fluence captured by FluenceOfXAndYAndZ detector: ' num2str(sum(results{di}.FluenceOfXAndYAndZ.Mean(:)*xyznorm))]);
        end
    end
    if isfield(results{di}, 'RadianceOfRhoAndZAndAngle') && show.RadianceOfRhoAndZAndAngle
        numrhos = length(results{di}.RadianceOfRhoAndZAndAngle.Rho) - 1;
        numangles = length(results{di}.RadianceOfRhoAndZAndAngle.Angle) - 1;
        numzs = length(results{di}.RadianceOfRhoAndZAndAngle.Z) - 1;
        for i=1:numangles
            figname = sprintf('log(%s) %5.3f<angle<%5.3f',results{di}.RadianceOfRhoAndZAndAngle.Name,(i-1)*pi/numangles,i*pi/numangles); 
            figure; imagesc(results{di}.RadianceOfRhoAndZAndAngle.Z_Midpoints, results{di}.RadianceOfRhoAndZAndAngle.Rho_Midpoints, log(results{di}.RadianceOfRhoAndZAndAngle.Mean(:,:,i)')); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]');
        end
        rhodelta = results{di}.RadianceOfRhoAndZAndAngle.Rho(2)-results{di}.RadianceOfRhoAndZAndAngle.Rho(1);
        zdelta = results{di}.RadianceOfRhoAndZAndAngle.Z(2)-results{di}.RadianceOfRhoAndZAndAngle.Z(1);
        angledelta = results{di}.RadianceOfRhoAndZAndAngle.Angle(2)-results{di}.RadianceOfRhoAndZAndAngle.Angle(1);
        rhonorm = 2 * pi * results{di}.RadianceOfRhoAndZAndAngle.Rho_Midpoints * rhodelta;
        anglenorm = 2 * pi * sin(results{di}.RadianceOfRhoAndZAndAngle.Angle_Midpoints * angledelta) * angledelta;
        anglematrix = repmat(anglenorm',[1,numrhos,numzs]);
        disp(['Radiance captured by RadianceOfRhoAndZAndAngle detector: ' num2str(sum(sum(sum(zdelta*results{di}.RadianceOfRhoAndZAndAngle.Mean.*repmat(rhonorm',[1,numzs,numangles]).*permute(anglematrix,[2,3,1])))))]);
    end
    if isfield(results{di}, 'RadianceOfXAndYAndZAndThetaAndPhi') && show.RadianceOfXAndYAndZAndThetaAndPhi
        numTheta = length(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta) - 1;      
        % plot radiance vs x and z for each theta (polar angle from Uz=[-1:1]
        for i=1:numTheta
            figname = sprintf('log(%s) %5.3f<Theta<%5.3f',results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Name,(i-1)*pi/numTheta,i*pi/numTheta); 
            figure; imagesc(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X_Midpoints, results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z_Midpoints, log(squeeze(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Mean(:,1,:,i,1))'), [-20 -5]); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]');
        end
        xyzthetaphinorm = (results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Y(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Y(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Phi(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Phi(1));    
        disp(['Radiance captured by RadianceOfXAndYAndZAndThetaAndPhi detector: ' num2str(sum(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Mean(:)*xyzthetaphinorm))]);
    end
    if isfield(results{di}, 'ReflectedMTOfRhoAndSubregionHist') && show.ReflectedMTOfRhoAndSubregionHist
        numrhos = length(results{di}.ReflectedMTOfRhoAndSubregionHist.Rho) - 1;
        numsubregions = length(results{di}.ReflectedMTOfRhoAndSubregionHist.SubregionIndices);
        figname = sprintf('log(%s)',results{di}.ReflectedMTOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.ReflectedMTOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints, log(results{di}.ReflectedMTOfRhoAndSubregionHist.Mean'));...        
           colorbar; title(figname); xlabel('\rho [mm]'); ylabel('MT'); set(gcf,'Name', figname);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        for j=2:3 % customized, general form: j=1:numsubregions
        for i=1:20:41 % customized, general form: i=1:numrhos
            %figure; plot(results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints,results{di}.ReflectedMTOfRhoAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Fractional MT in Region %2d, Rho = %5.3f mm',j-1,results{di}.ReflectedMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            X=results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints;
            %layerfrac=squeeze(results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(i,:,j,:));
            %bar(X,layerfrac,'stacked'); title(figname);xlabel('MT'),ylabel('photon weight');
            stack=zeros(size(results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(i,:,j,1)));
            for k=1:size(results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT,4)                
                stack=stack+results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(i,:,j,k);
                semilogy(X,stack,color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('MT'),ylabel('stacked log(photon weight)'); hold on;
            end
            legend('[0-0.1]','[0.1-0.2]','[0.2-0.3]','[0.3-0.4]','[0.4-0.5]','[0.5-0.6]','[0.6-0.7]','[0.7-0.8]','[0.8-0.9]','[0.9,1]'); % customized
        end
        end
    end
    if isfield(results{di}, 'ReflectedTimeOfRhoAndSubregionHist') && show.ReflectedTimeOfRhoAndSubregionHist
        numtissueregions = length(results{di}.ReflectedTimeOfRhoAndSubregionHist.SubregionIndices);
        for i=1:numtissueregions
            figname = sprintf('log(%s) Region Index %d',results{di}.ReflectedTimeOfRhoAndSubregionHist.Name, i-1); 
            figure; imagesc(results{di}.ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedTimeOfRhoAndSubregionHist.Time_Midpoints, log(squeeze(results{di}.ReflectedTimeOfRhoAndSubregionHist.Mean(:,i,:))'));       
               colorbar; caxis([-15 0]);title(figname); set(gcf,'Name', figname); ylabel('time [ns]'); xlabel('\rho [mm]');
        end
        figname = sprintf('%s Fractional Time',results{di}.ReflectedTimeOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedTimeOfRhoAndSubregionHist.SubregionIndices-1, results{di}.ReflectedTimeOfRhoAndSubregionHist.FractionalTime');       
               colorbar; title(figname); set(gcf,'Name', figname); ylabel('subregion index'); xlabel('\rho [mm]')
        disp(['Time in Subregion captured by ReflectedTimeOfRhoAndSubregionHist detector: ' num2str(sum(results{di}.ReflectedTimeOfRhoAndSubregionHist.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCROfRho') && show.pMCROfRho
        figname = sprintf('log(%s)',results{di}.pMCROfRho.Name); figure; plot(results{di}.pMCROfRho.Rho_Midpoints, log10(results{di}.pMCROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('pMC R(\rho) [mm^-^2]');
        disp(['Total reflectance captured by pMCROfRho detector: ' num2str(sum(results{di}.pMCROfRho.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCROfRhoAndTime') && show.pMCROfRhoAndTime
        figname = sprintf('log(%s)',results{di}.pMCROfRhoAndTime.Name); figure; imagesc(results{di}.pMCROfRhoAndTime.Rho_Midpoints, results{di}.pMCROfRhoAndTime.Time_Midpoints,log(results{di}.pMCROfRhoAndTime.Mean')); colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('\rho [mm]');
        disp(['Total reflectance captured by pMCROfRhoAndTime detector: ' num2str(sum(results{di}.pMCROfRhoAndTime.Mean(:)))]);
    end
  end
end