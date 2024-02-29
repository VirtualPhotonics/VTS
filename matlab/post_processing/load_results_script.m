% script for loading Monte Carlo results
clear variables;
dbstop if error;
slash = filesep;  % get correct path delimiter for platform

% script to parse results from MC simulation
addpath(pwd);
addpath([pwd slash 'jsonlab']);

% names of individual MC simulations
datanames = { 'one_layer_all_detectors' };
% datanames = { 'results_mua0.1musp1.0' 'results_mua0.1musp1.1' }; %...etc

% outdir = 'C:\Projects\vts\src\Vts.MonteCarlo.CommandLineApplication\bin\Release';
outdir = '.';

show.SurfaceFiber =             1;
show.SlantedRecessedFiber =     1;
show.RDiffuse =                 1;
show.ROfRho =                   1;
show.ROfRhoRecessed =           1;
show.ROfAngle =                 1;
show.ROfRhoAndTime =            1;
show.ROfRhoAndMaxDepth =        1;
show.ROfRhoAndMaxDepthRecessed =1;
show.ROfRhoAndAngle =           1;
show.ROfRhoAndOmega =           1;
show.ROfXAndY =                 1;
show.ROfXAndYRecessed =         1;
show.ROfXAndYAndTime =          1;
show.ROfXAndYAndTimeRecessed =  1;
show.ROfXAndYAndTimeAndSubregion =          1;
show.ROfXAndYAndTimeAndSubregionRecessed =  1;
show.ROfXAndYAndThetaAndPhi =   1;
show.ROfXAndYAndMaxDepth =      1;
show.ROfXAndYAndMaxDepthRecessed=1;
show.ROfFx =                    1;
show.ROfFxAndTime =             1;
show.ROfFxAndAngle =            1;
show.ROfFxAndMaxDepth =         1;
show.RSpecular =                1;
show.TDiffuse =                 1;
show.TOfRho =                   1;
show.TOfRhoAndAngle =           1;
show.TOfAngle =                 1;
show.TOfXAndY =                 1;
show.TOfXAndYAndTimeAndSubregion =          1;
show.TOfFx =					1;
show.ATotal =                   1;
show.AOfRhoAndZ =               1;
show.AOfXAndYAndZ = 			1;
show.ATotalBoundingVolume =     1;
show.FluenceOfRhoAndZ =         1;
show.FluenceOfRhoAndZAndTime =  1;
show.FluenceOfRhoAndZAndOmega = 1;
show.FluenceOfXAndYAndZ =       1;
show.FluenceOfXAndYAndZAndTime =  1;
show.FluenceOfXAndYAndZAndOmega =  1;
show.FluenceOfXAndYAndZAndStartingXAndY = 1;
show.FluenceOfFxAndZ =          1;
show.RadianceOfRhoAndZAndAngle = 1;
show.RadianceOfFxAndZAndAngle = 1;
show.RadianceOfXAndYAndZAndThetaAndPhi = 1;
show.pMCATotal =                1;
show.pMCROfRho =                1;
show.pMCROfRhoRecessed =        1;
show.pMCROfRhoAndTime =         1;
show.pMCROfRhoAndTimeRecessed = 1;
show.pMCROfXAndY =              1;
show.pMCROfXAndYAndTimeAndSubregion = 1;
show.pMCROfXAndYAndTimeAndSubregionRecessed = 1;
show.pMCROfFx =                 1;
show.pMCTOfRho =                1;
show.ReflectedMTOfRhoAndSubregionHist = 1;
show.ReflectedMTOfXAndYAndSubregionHist = 1;
show.TransmittedMTOfRhoAndSubregionHist = 1;
show.TransmittedMTOfXAndYAndSubregionHist = 1;
show.ReflectedDynamicMTOfRhoAndSubregionHist = 1;
show.ReflectedDynamicMTOfXAndYAndSubregionHist = 1;
show.ReflectedDynamicMTOfFxAndSubregionHist = 1;
show.TransmittedDynamicMTOfRhoAndSubregionHist = 1;
show.TransmittedDynamicMTOfXAndYAndSubregionHist = 1;
show.TransmittedDynamicMTOfFxAndSubregionHist = 1;
show.ReflectedTimeOfRhoAndSubregionHist = 1;

for mci = 1:length(datanames)
  dataname = datanames{mci};
  results = loadMCResults(outdir, dataname);  
  for di = 1:size(results, 2)
      
    if isfield(results{di}, 'SurfaceFiber') && show.SurfaceFiber
        disp(['Total reflectance captured by SurfaceFiber detector: ' num2str(results{di}.SurfaceFiber.Mean)]);
        disp(['Standard Deviation captured by SurfaceFiber detector: ' num2str(results{di}.SurfaceFiber.Stdev)]);
        disp(['+/- 3sigma by SurfaceFiber detector: ' ...
            num2str(results{di}.SurfaceFiber.Mean - 3 * results{di}.SurfaceFiber.Stdev) ' - ' ...
            num2str(results{di}.SurfaceFiber.Mean + 3 * results{di}.SurfaceFiber.Stdev)]);
    end
        
    if isfield(results{di}, 'SlantedRecessedFiber') && show.SlantedRecessedFiber
        disp(['Total reflectance captured by SlantedRecessedFiber detector: ' num2str(results{di}.SlantedRecessedFiber.Mean)]);
        disp(['Standard Deviation captured by SlantedRecessedFiber detector: ' num2str(results{di}.SlantedRecessedFiber.Stdev)]);
        disp(['+/- 3sigma by SlantedRecessedFiber detector: ' ...
            num2str(results{di}.SlantedRecessedFiber.Mean - 3 * results{di}.SlantedRecessedFiber.Stdev) ' - ' ...
            num2str(results{di}.SlantedRecessedFiber.Mean + 3 * results{di}.SlantedRecessedFiber.Stdev)]);
    end

    if isfield(results{di}, 'RDiffuse') && show.RDiffuse
        disp(['Total reflectance captured by RDiffuse detector: ' num2str(results{di}.RDiffuse.Mean)]);
    end
    
    if isfield(results{di}, 'RSpecular') && show.RSpecular
        disp(['Total reflectance captured by RSpecular detector: ' num2str(results{di}.RSpecular.Mean)]);
    end
    
    if isfield(results{di}, 'ROfRho') && show.ROfRho
        figname = sprintf('log10(%s)',results{di}.ROfRho.Name); figure; plot(results{di}.ROfRho.Rho_Midpoints, log10(results{di}.ROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('R(\rho) [mm^-^2]');
        rhodelta = results{di}.ROfRho.Rho(2)-results{di}.ROfRho.Rho(1);
        rhonorm = 2 * pi * results{di}.ROfRho.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(results{di}.ROfRho.Mean.*rhonorm'))]);
    end
    
    if isfield(results{di}, 'ROfRhoRecessed') && show.ROfRhoRecessed
        figname = sprintf('log10(%s)',results{di}.ROfRhoRecessed.Name); figure; plot(results{di}.ROfRhoRecessed.Rho_Midpoints, log10(results{di}.ROfRhoRecessed.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('R(\rho) [mm^-^2]');
        rhodelta = results{di}.ROfRhoRecessed.Rho(2)-results{di}.ROfRhoRecessed.Rho(1);
        rhonorm = 2 * pi * results{di}.ROfRhoRecessed.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by ROfRhoRecessed detector: ' num2str(sum(results{di}.ROfRhoRecessed.Mean.*rhonorm'))]);
    end
    
    if isfield(results{di}, 'ROfAngle') && show.ROfAngle
        figname = sprintf('log10(%s)',results{di}.ROfAngle.Name); figure; plot(results{di}.ROfAngle.Angle_Midpoints, log10(results{di}.ROfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('R(angle) [rad^-^1]');
        angledelta = results{di}.ROfAngle.Angle(2)-results{di}.ROfAngle.Angle(1);
        anglenorm = 2 * pi * sin(results{di}.ROfAngle.Angle_Midpoints) * angledelta;
        disp(['Total reflectance captured by ROfAngle detector: ' num2str(sum(results{di}.ROfAngle.Mean.*anglenorm'))]);
    end

    if isfield(results{di}, 'ROfRhoAndTime') && show.ROfRhoAndTime
        numtimes = length(results{di}.ROfRhoAndTime.Time)-1;
        figname = sprintf('log10(%s)',results{di}.ROfRhoAndTime.Name); figure; imagesc(results{di}.ROfRhoAndTime.Rho_Midpoints, results{di}.ROfRhoAndTime.Time_Midpoints,log10(results{di}.ROfRhoAndTime.Mean)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('time [ns]'); xlabel('\rho [mm]');
        rhodelta = results{di}.ROfRhoAndTime.Rho(2)-results{di}.ROfRhoAndTime.Rho(1);
        timedelta = results{di}.ROfRhoAndTime.Time(2)-results{di}.ROfRhoAndTime.Time(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndTime.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(sum(sum(timedelta*results{di}.ROfRhoAndTime.Mean.*repmat(rhonorm,[numtimes,1]))))]);
    end

    if isfield(results{di}, 'ROfRhoAndMaxDepth') && show.ROfRhoAndMaxDepth
        numrhos = length(results{di}.ROfRhoAndMaxDepth.Rho)-1;
        numdepths = length(results{di}.ROfRhoAndMaxDepth.MaxDepth)-1;
        figname = sprintf('log10(%s)',results{di}.ROfRhoAndMaxDepth.Name); figure; imagesc(results{di}.ROfRhoAndMaxDepth.Rho_Midpoints, results{di}.ROfRhoAndMaxDepth.MaxDepth_Midpoints,log10(results{di}.ROfRhoAndMaxDepth.Mean)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('max depths [mm]'); xlabel('\rho [mm]');
        rhodelta = results{di}.ROfRhoAndMaxDepth.Rho(2)-results{di}.ROfRhoAndMaxDepth.Rho(1);
        depthdelta = results{di}.ROfRhoAndMaxDepth.MaxDepth(2)-results{di}.ROfRhoAndMaxDepth.MaxDepth(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndMaxDepth.Rho_Midpoints * rhodelta;
        % plot distribution for select rhow
        figname = 'Max Depth Distribution';figure;
        k=1; % index for legend
        for i=1:10:numrhos % do every 10 rhos
            plot(results{di}.ROfRhoAndMaxDepth.MaxDepth_Midpoints,results{di}.ROfRhoAndMaxDepth.Mean(:,i));
            br{k}=sprintf('rho=%s',results{di}.ROfRhoAndMaxDepth.Rho_Midpoints(i));
            hold on;
            k=k+1;
        end
        legend(br);
        title(figname);xlabel('z [mm]');ylabel('max depth');
        disp(['Total reflectance captured by ROfRhoAndMaxDepth detector: ' num2str(sum(sum(results{di}.ROfRhoAndMaxDepth.Mean.*repmat(rhonorm,[numdepths,1]))))]);
    end
    
    if isfield(results{di}, 'ROfRhoAndMaxDepthRecessed') && show.ROfRhoAndMaxDepthRecessed
        numrhos = length(results{di}.ROfRhoAndMaxDepthRecessed.Rho)-1;
        numdepths = length(results{di}.ROfRhoAndMaxDepthRecessed.MaxDepth)-1;
        figname = sprintf('log10(%s)',results{di}.ROfRhoAndMaxDepthRecessed.Name); figure; imagesc(results{di}.ROfRhoAndMaxDepthRecessed.Rho_Midpoints, results{di}.ROfRhoAndMaxDepthRecessed.MaxDepth_Midpoints,log10(results{di}.ROfRhoAndMaxDepthRecessed.Mean)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('max depths [mm]'); xlabel('\rho [mm]');
        rhodelta = results{di}.ROfRhoAndMaxDepthRecessed.Rho(2)-results{di}.ROfRhoAndMaxDepthRecessed.Rho(1);
        depthdelta = results{di}.ROfRhoAndMaxDepthRecessed.MaxDepth(2)-results{di}.ROfRhoAndMaxDepthRecessed.MaxDepth(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndMaxDepthRecessed.Rho_Midpoints * rhodelta;
        % plot distribution for select rhow
        figname = 'Max Depth Distribution';figure;
        k=1; % index for legend
        for i=1:10:numrhos % do every 10 rhos
            plot(results{di}.ROfRhoAndMaxDepthRecessed.MaxDepth_Midpoints,results{di}.ROfRhoAndMaxDepthRecessed.Mean(:,i));
            br{k}=sprintf('rho=%s',results{di}.ROfRhoAndMaxDepthRecessed.Rho_Midpoints(i));
            hold on;
            k=k+1;
        end
        legend(br);
        title(figname);xlabel('z [mm]');ylabel('max depth');
        disp(['Total reflectance captured by ROfRhoAndMaxDepthRecessed detector: ' num2str(sum(sum(results{di}.ROfRhoAndMaxDepthRecessed.Mean.*repmat(rhonorm,[numdepths,1]))))]);
    end
    if isfield(results{di}, 'ROfRhoAndAngle') && show.ROfRhoAndAngle        
        figname = sprintf('log10(%s)',results{di}.ROfRhoAndAngle.Name); figure; imagesc(results{di}.ROfRhoAndAngle.Rho_Midpoints, results{di}.ROfRhoAndAngle.Angle_Midpoints, log10(results{di}.ROfRhoAndAngle.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('\angle [rad]'); xlabel('\rho [mm]'); 
        rhodelta = results{di}.ROfRhoAndAngle.Rho(2)-results{di}.ROfRhoAndAngle.Rho(1);
        angledelta = results{di}.ROfRhoAndAngle.Angle(2)-results{di}.ROfRhoAndAngle.Angle(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndAngle.Rho_Midpoints * rhodelta;
        anglenorm = 2 * pi * sin(results{di}.ROfRhoAndAngle.Angle_Midpoints) * angledelta;
        disp(['Total reflectance captured by ROfRhoAndAngle detector: ' num2str(sum(sum(results{di}.ROfRhoAndAngle.Mean.*repmat(anglenorm',[1,size(rhonorm,2)]).*repmat(rhonorm,[size(anglenorm,2),1]))))]);
    end

    if isfield(results{di}, 'ROfRhoAndOmega') && show.ROfRhoAndOmega
        figname = sprintf('%s - log10(Amplitude)',results{di}.ROfRhoAndOmega.Name); figure; imagesc(results{di}.ROfRhoAndOmega.Rho_Midpoints, results{di}.ROfRhoAndOmega.Omega_Midpoints, log10(results{di}.ROfRhoAndOmega.Amplitude)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('\omega [GHz]'); xlabel('\rho [mm]');
        figname = sprintf('%s - Phase',results{di}.ROfRhoAndOmega.Name); figure; imagesc(results{di}.ROfRhoAndOmega.Rho_Midpoints, results{di}.ROfRhoAndOmega.Omega_Midpoints, results{di}.ROfRhoAndOmega.Phase); colorbar; title(figname); set(gcf,'Name', figname); ylabel('\omega [GHz]'); xlabel('\rho [mm]');
        rhodelta = results{di}.ROfRhoAndOmega.Rho(2)-results{di}.ROfRhoAndOmega.Rho(1);
        rhonorm = 2 * pi * results{di}.ROfRhoAndOmega.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by ROfRhoAndOmega detector: ' num2str(sum(results{di}.ROfRhoAndOmega.Amplitude(1,:).*rhonorm))]);
    end    
    
    if isfield(results{di}, 'ROfXAndY') && show.ROfXAndY
        if (length(results{di}.ROfXAndY.Y_Midpoints)>1)
          figname = sprintf('log10(%s)',results{di}.ROfXAndY.Name); figure; imagesc(results{di}.ROfXAndY.Y_Midpoints, results{di}.ROfXAndY.X_Midpoints, log10(results{di}.ROfXAndY.Mean)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('Y [mm]'); xlabel('X [mm]');
        else
          figname = 'log10(R(x,y=0)';figure;plot(results{di}.ROfXAndY.X_Midpoints,results{di}.ROfXAndY.Mean(1,:));title(figname);xlabel('X [mm]');ylabel('R(x) [mm^-^2]');
        end
        xynorm = (results{di}.ROfXAndY.X(2)-results{di}.ROfXAndY.X(1))*(results{di}.ROfXAndY.Y(2)-results{di}.ROfXAndY.Y(1));
        disp(['Total reflectance captured by ROfXAndY detector: ' num2str(sum(results{di}.ROfXAndY.Mean(:)*xynorm))]);
        % determine range of x, y midpoints that have non-zero data
        [r,c]=find(results{di}.ROfXAndY.Mean);
        disp(sprintf('ROfXAndY: x non-zero span [%d %d]',min(r),max(r))); disp(sprintf('ROfXAndY: y non-zero span [%d %d]',min(c),max(c)));
    end
    if isfield(results{di}, 'ROfXAndYRecessed') && show.ROfXAndYRecessed
        figname = sprintf('log10(%s)',results{di}.ROfXAndYRecessed.Name); figure; imagesc(results{di}.ROfXAndYRecessed.Y_Midpoints, results{di}.ROfXAndYRecessed.X_Midpoints, log10(results{di}.ROfXAndYRecessed.Mean)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('Y [mm]'); xlabel('X [mm]');
        xynorm = (results{di}.ROfXAndYRecessed.X(2)-results{di}.ROfXAndYRecessed.X(1))*(results{di}.ROfXAndYRecessed.Y(2)-results{di}.ROfXAndYRecessed.Y(1));
        disp(['Total reflectance captured by ROfXAndYRecessed detector: ' num2str(sum(results{di}.ROfXAndYRecessed.Mean(:)*xynorm))]);
        % determine range of x, y midpoints that have non-zero data
        [r,c]=find(results{di}.ROfXAndYRecessed.Mean);
        disp(sprintf('ROfXAndYRecessed: x non-zero span [%d %d]',min(r),max(r))); disp(sprintf('ROfXAndYRecessed: y non-zero span [%d %d]',min(c),max(c)));
    end
    
    if isfield(results{di}, 'ROfXAndYAndTime') && show.ROfXAndYAndTime
        numtimes = length(results{di}.ROfXAndYAndTime.Time)-1;
        for i=1:10:numtimes % do every 10 time bins
            figname = sprintf('log10(%s) time=%5.3f ns',results{di}.ROfXAndYAndTime.Name,results{di}.ROfXAndYAndTime.Time_Midpoints(i)); 
            figure; imagesc(results{di}.ROfXAndYAndTime.X_Midpoints, results{di}.ROfXAndYAndTime.Y_Midpoints, log10(squeeze(results{di}.ROfXAndYAndTime.Mean(i,:,:)))); 
            colormap(jet); colorbar; title(figname); set(gcf,'Name', figname);ylabel('y [mm]'); xlabel('x [mm]'); 
        end
        xdelta = results{di}.ROfXAndYAndTime.X(2)-results{di}.ROfXAndYAndTime.X(1);        
        ydelta = results{di}.ROfXAndYAndTime.Y(2)-results{di}.ROfXAndYAndTime.Y(1);
        timedelta = results{di}.ROfXAndYAndTime.Time(2)-results{di}.ROfXAndYAndTime.Time(1);
        xynorm = xdelta * ydelta;
        disp(['Total reflectance captured by ROfXAndYAndTime detector: ' num2str(sum(sum(sum(timedelta*xynorm*results{di}.ROfXAndYAndTime.Mean))))]);
    end
    if isfield(results{di}, 'ROfXAndYAndTimeRecessed') && show.ROfXAndYAndTimeRecessed
        numtimes = length(results{di}.ROfXAndYAndTimeRecessed.Time)-1;
        for i=1:10:numtimes % do every 10 time bins
            figname = sprintf('log10(%s) time=%5.3f ns',results{di}.ROfXAndYAndTimeRecessed.Name,results{di}.ROfXAndYAndTimeRecessed.Time_Midpoints(i)); 
            figure; imagesc(results{di}.ROfXAndYAndTimeRecessed.X_Midpoints, results{di}.ROfXAndYAndTimeRecessed.Y_Midpoints, log10(squeeze(results{di}.ROfXAndYAndTimeRecessed.Mean(i,:,:)))); 
            colormap(jet); colorbar; title(figname); set(gcf,'Name', figname);ylabel('y [mm]'); xlabel('x [mm]'); 
        end
        xdelta = results{di}.ROfXAndYAndTimeRecessed.X(2)-results{di}.ROfXAndYAndTimeRecessed.X(1);        
        ydelta = results{di}.ROfXAndYAndTimeRecessed.Y(2)-results{di}.ROfXAndYAndTimeRecessed.Y(1);
        timedelta = results{di}.ROfXAndYAndTimeRecessed.Time(2)-results{di}.ROfXAndYAndTimeRecessed.Time(1);
        xynorm = xdelta * ydelta;
        disp(['Total reflectance captured by ROfXAndYAndTimeRecessed detector: ' num2str(sum(sum(sum(timedelta*xynorm*results{di}.ROfXAndYAndTimeRecessed.Mean))))]);
    end
    if isfield(results{di}, 'ROfXAndYAndTimeAndSubregion') && show.ROfXAndYAndTimeAndSubregion
        y0idx = floor(length(results{di}.ROfXAndYAndTimeAndSubregion.Y_Midpoints)/2);
        for i=2:results{di}.ROfXAndYAndTimeAndSubregion.NumberOfRegions-1 % exclude air above and below          
          figname = sprintf('log10(%s) region idx=%i',results{di}.ROfXAndYAndTimeAndSubregion.Name,i-1); figure; 
          imagesc(results{di}.ROfXAndYAndTimeAndSubregion.X_Midpoints, results{di}.ROfXAndYAndTimeAndSubregion.Time_Midpoints,...
            log10(squeeze(results{di}.ROfXAndYAndTimeAndSubregion.Mean(i,:,y0idx,:)))); 
          colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('x [mm]');
        end
        xdelta = results{di}.ROfXAndYAndTimeAndSubregion.X(2)-results{di}.ROfXAndYAndTimeAndSubregion.X(1);        
        ydelta = results{di}.ROfXAndYAndTimeAndSubregion.Y(2)-results{di}.ROfXAndYAndTimeAndSubregion.Y(1);
        timedelta = results{di}.ROfXAndYAndTimeAndSubregion.Time(2)-results{di}.ROfXAndYAndTimeAndSubregion.Time(1);
        % the following does not integrate to diffuse R
        disp(['Total reflectance captured by ROfXAndYAndTimeAndSubregion detector: ' num2str(sum(xdelta*ydelta*timedelta*results{di}.ROfXAndYAndTimeAndSubregion.Mean(:)))]);
        % but this does
        disp(['Total reflectance captured by ROfXAndYAndTimeAndSubregion detector - ROfXAndY: ' num2str(sum(xdelta*ydelta*results{di}.ROfXAndYAndTimeAndSubregion.ROfXAndY(:)))]);
    end
    if isfield(results{di}, 'ROfXAndYAndTimeAndSubregionRecessed') && show.ROfXAndYAndTimeAndSubregionRecessed
        y0idx = floor(length(results{di}.ROfXAndYAndTimeAndSubregionRecessed.Y_Midpoints)/2);
        for i=2:results{di}.ROfXAndYAndTimeAndSubregionRecessed.NumberOfRegions-1 % exclude air above and below          
          figname = sprintf('log10(%s) region idx=%i',results{di}.ROfXAndYAndTimeAndSubregionRecessed.Name,i-1); figure; 
          imagesc(results{di}.ROfXAndYAndTimeAndSubregionRecessed.X_Midpoints, results{di}.ROfXAndYAndTimeAndSubregionRecessed.Time_Midpoints,...
            log10(squeeze(results{di}.ROfXAndYAndTimeAndSubregionRecessed.Mean(i,:,y0idx,:)))); 
          colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('x [mm]');
        end
        xdelta = results{di}.ROfXAndYAndTimeAndSubregionRecessed.X(2)-results{di}.ROfXAndYAndTimeAndSubregionRecessed.X(1);        
        ydelta = results{di}.ROfXAndYAndTimeAndSubregionRecessed.Y(2)-results{di}.ROfXAndYAndTimeAndSubregionRecessed.Y(1);
        timedelta = results{di}.ROfXAndYAndTimeAndSubregionRecessed.Time(2)-results{di}.ROfXAndYAndTimeAndSubregionRecessed.Time(1);
        % the following does not integrate to diffuse R
        disp(['Total reflectance captured by ROfXAndYAndTimeAndSubregionRecessed detector: ' num2str(sum(xdelta*ydelta*timedelta*results{di}.ROfXAndYAndTimeAndSubregionRecessed.Mean(:)))]);
        % but this does
        disp(['Total reflectance captured by ROfXAndYAndTimeAndSubregionRecessed detector - ROfXAndY: ' num2str(sum(xdelta*ydelta*results{di}.ROfXAndYAndTimeAndSubregionRecessed.ROfXAndY(:)))]);
    end
    if isfield(results{di}, 'ROfXAndYAndThetaAndPhi') && show.ROfXAndYAndThetaAndPhi
        yidx = floor(length(results{di}.ROfXAndYAndThetaAndPhi.Y_Midpoints) / 2);
        xidx = floor(length(results{di}.ROfXAndYAndThetaAndPhi.X_Midpoints) / 2);
        figname = sprintf('log10(%s) x=%5.3f y=%5.3f',results{di}.ROfXAndYAndThetaAndPhi.Name,...
            results{di}.ROfXAndYAndThetaAndPhi.X(xidx),results{di}.ROfXAndYAndThetaAndPhi.Y(yidx)); 
        figure; imagesc(results{di}.ROfXAndYAndThetaAndPhi.Phi_Midpoints, results{di}.ROfXAndYAndThetaAndPhi.Theta_Midpoints, ...
            log10(squeeze(results{di}.ROfXAndYAndThetaAndPhi.Mean(:,:,yidx,xidx)))); 
        colormap(jet); colorbar; title(figname); set(gcf,'Name', figname);ylabel('theta [radians]'); xlabel('phi [radians]');         
        xdelta = results{di}.ROfXAndYAndThetaAndPhi.X(2)-results{di}.ROfXAndYAndThetaAndPhi.X(1);        
        ydelta = results{di}.ROfXAndYAndThetaAndPhi.Y(2)-results{di}.ROfXAndYAndThetaAndPhi.Y(1);
        thetadelta = results{di}.ROfXAndYAndThetaAndPhi.Theta(2)-results{di}.ROfXAndYAndThetaAndPhi.Theta(1);
        phidelta = results{di}.ROfXAndYAndThetaAndPhi.Phi(2)-results{di}.ROfXAndYAndThetaAndPhi.Phi(1);
        xyphinorm = xdelta * ydelta * phidelta;
        partialsum = xyphinorm * sum(sum(sum(results{di}.ROfXAndYAndThetaAndPhi.Mean,1),3),4);
        thetanorm = sin(results{di}.ROfXAndYAndThetaAndPhi.Theta_Midpoints) * thetadelta;  
        disp(['Total reflectance captured by ROfXAndYAndThetaAndPhi detector: ' num2str(sum(partialsum.*thetanorm))]);
      end
    if isfield(results{di}, 'ROfXAndYAndMaxDepth') && show.ROfXAndYAndMaxDepth
        numdepths = length(results{di}.ROfXAndYAndMaxDepth.MaxDepth)-1;
        for i=1:10:numdepths % do every 10 depth bins
            figname = sprintf('log10(%s) depth=%5.3f mm',results{di}.ROfXAndYAndMaxDepth.Name,results{di}.ROfXAndYAndMaxDepth.MaxDepth_Midpoints(i)); 
            figure; imagesc(results{di}.ROfXAndYAndMaxDepth.X_Midpoints, results{di}.ROfXAndYAndMaxDepth.Y_Midpoints, log10(squeeze(results{di}.ROfXAndYAndMaxDepth.Mean(i,:,:)))); 
            colormap(jet); colorbar; title(figname); set(gcf,'Name', figname);ylabel('y [mm]'); xlabel('x [mm]'); 
        end
        xdelta = results{di}.ROfXAndYAndMaxDepth.X(2)-results{di}.ROfXAndYAndMaxDepth.X(1);        
        ydelta = results{di}.ROfXAndYAndMaxDepth.Y(2)-results{di}.ROfXAndYAndMaxDepth.Y(1);
        timedelta = results{di}.ROfXAndYAndMaxDepth.MaxDepth(2)-results{di}.ROfXAndYAndMaxDepth.MaxDepth(1);
        xynorm = xdelta * ydelta;
        disp(['Total reflectance captured by ROfXAndYAndMaxDepth detector: ' num2str(sum(sum(sum(timedelta*xynorm*results{di}.ROfXAndYAndMaxDepth.Mean))))]);
    end
    if isfield(results{di}, 'ROfXAndYAndMaxDepthRecessed') && show.ROfXAndYAndMaxDepthRecessed
        numdepths = length(results{di}.ROfXAndYAndMaxDepthRecessed.MaxDepth)-1;
        for i=1:10:numdepths % do every 10 depth bins
            figname = sprintf('log10(%s) depth=%5.3f mm',results{di}.ROfXAndYAndMaxDepthRecessed.Name,results{di}.ROfXAndYAndMaxDepthRecessed.MaxDepth_Midpoints(i)); 
            figure; imagesc(results{di}.ROfXAndYAndMaxDepthRecessed.X_Midpoints, results{di}.ROfXAndYAndMaxDepthRecessed.Y_Midpoints, log10(squeeze(results{di}.ROfXAndYAndMaxDepthRecessed.Mean(i,:,:)))); 
            colormap(jet); colorbar; title(figname); set(gcf,'Name', figname);ylabel('y [mm]'); xlabel('x [mm]'); 
        end
        xdelta = results{di}.ROfXAndYAndMaxDepthRecessed.X(2)-results{di}.ROfXAndYAndMaxDepthRecessed.X(1);        
        ydelta = results{di}.ROfXAndYAndMaxDepthRecessed.Y(2)-results{di}.ROfXAndYAndMaxDepthRecessed.Y(1);
        timedelta = results{di}.ROfXAndYAndMaxDepthRecessed.MaxDepth(2)-results{di}.ROfXAndYAndMaxDepthRecessed.MaxDepth(1);
        xynorm = xdelta * ydelta;
        disp(['Total reflectance captured by ROfXAndYAndMaxDepthRecessed detector: ' num2str(sum(sum(sum(timedelta*xynorm*results{di}.ROfXAndYAndMaxDepthRecessed.Mean))))]);
    end
    
    if isfield(results{di}, 'ROfFx') && show.ROfFx
        figname = sprintf('%s',results{di}.ROfFx.Name); figure; plot(results{di}.ROfFx.Fx_Midpoints, abs(results{di}.ROfFx.Mean)); title(figname); set(gcf,'Name', figname); 
        xlabel('f_x [/mm]'); ylabel('R(f_x) [unitless]');
        disp(['Total reflectance captured by ROfFx detector: ' num2str(results{di}.ROfFx.Amplitude(1))]);
    end

    if isfield(results{di}, 'ROfFxAndTime') && show.ROfFxAndTime
        figname = sprintf('%s - log10(Amplitude)',results{di}.ROfFxAndTime.Name); 
        figure; imagesc(results{di}.ROfFxAndTime.Time_Midpoints, results{di}.ROfFxAndTime.Fx_Midpoints, log10(results{di}.ROfFxAndTime.Amplitude')); 
        title(figname); set(gcf,'Name', figname);colorbar; xlabel('time [ns]'); ylabel('f_x [/mm]');
        timedelta = results{di}.ROfFxAndTime.Time(2)-results{di}.ROfFxAndTime.Time(1);
        disp(['Total reflectance captured by ROfFxAndTime detector: ' num2str(sum(timedelta*results{di}.ROfFxAndTime.Amplitude(:,1)))]);
        end
 
    if isfield(results{di}, 'ROfFxAndAngle') && show.ROfFxAndAngle
        figname = sprintf('%s - log10(Amplitude)',results{di}.ROfFxAndAngle.Name); 
        figure; imagesc(results{di}.ROfFxAndAngle.Angle_Midpoints, results{di}.ROfFxAndAngle.Fx_Midpoints, log10(results{di}.ROfFxAndAngle.Amplitude')); 
        title(figname); set(gcf,'Name', figname);colorbar; xlabel('angle [rad]'); ylabel('f_x [/mm]');
        angledelta = results{di}.ROfFxAndAngle.Angle(2)-results{di}.ROfFxAndAngle.Angle(1);
        anglenorm = 2 * pi * sin(results{di}.ROfFxAndAngle.Angle_Midpoints) * angledelta;
        disp(['Total reflectance captured by ROfFxAndAngle detector: ' num2str(sum(anglenorm*results{di}.ROfFxAndAngle.Amplitude(:,1)))]);
    end
    
    if isfield(results{di}, 'ROfFxAndMaxDepth') && show.ROfFxAndMaxDepth
        figname = sprintf('%s - log10(Amplitude)',results{di}.ROfFxAndMaxDepth.Name); 
        figure; imagesc(results{di}.ROfFxAndMaxDepth.MaxDepth_Midpoints, results{di}.ROfFxAndMaxDepth.Fx_Midpoints, log10(results{di}.ROfFxAndMaxDepth.Amplitude')); 
        title(figname); set(gcf,'Name', figname);colorbar; xlabel('depth [mm]'); ylabel('f_x [/mm]');
        angledelta = results{di}.ROfFxAndMaxDepth.MaxDepth(2)-results{di}.ROfFxAndMaxDepth.MaxDepth(1);
        disp(['Total reflectance captured by ROfFxAndMaxDepth detector: ' num2str(sum(results{di}.ROfFxAndMaxDepth.Amplitude(:,1)))]);
    end
        
    if isfield(results{di}, 'TDiffuse') && show.TDiffuse
        disp(['Total transmittance captured by TDiffuse detector: ' num2str(results{di}.TDiffuse.Mean)]);
    end
    if isfield(results{di}, 'TOfRho') && show.TOfRho
         figname = sprintf('log10(%s)',results{di}.TOfRho.Name); figure; plot(results{di}.TOfRho.Rho_Midpoints, log10(results{di}.TOfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('T(\rho) [mm^-^2]');
         rhodelta = results{di}.TOfRho.Rho(2)-results{di}.TOfRho.Rho(1);
         rhonorm = 2 * pi * results{di}.TOfRho.Rho_Midpoints * rhodelta;
         disp(['Total transmittance captured by TOfRho detector: ' num2str(sum(results{di}.TOfRho.Mean.*rhonorm'))]);
    end
    if isfield(results{di}, 'TOfAngle') && show.TOfAngle
        figname = sprintf('log10(%s)',results{di}.TOfAngle.Name); figure; plot(results{di}.TOfAngle.Angle_Midpoints, log10(results{di}.TOfAngle.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\angle [rad]'); ylabel('T(angle) [rad^-^1]');
        angledelta = results{di}.TOfAngle.Angle(2)-results{di}.TOfAngle.Angle(1);
        anglenorm = 2 * pi * sin(results{di}.TOfAngle.Angle_Midpoints) * angledelta;
        disp(['Total transmittance captured by TOfAngle detector: ' num2str(sum(results{di}.TOfAngle.Mean.*anglenorm'))]);
    end
    if isfield(results{di}, 'TOfRhoAndAngle') && show.TOfRhoAndAngle
        figname = sprintf('log10(%s)',results{di}.TOfRhoAndAngle.Name); figure; imagesc(results{di}.TOfRhoAndAngle.Rho_Midpoints, results{di}.TOfRhoAndAngle.Angle_Midpoints, log10(results{di}.TOfRhoAndAngle.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('\angle [rad]');xlabel('\rho [mm]');
        rhodelta = results{di}.TOfRhoAndAngle.Rho(2)-results{di}.TOfRhoAndAngle.Rho(1);
        angledelta = results{di}.TOfRhoAndAngle.Angle(2)-results{di}.TOfRhoAndAngle.Angle(1);
        rhonorm = 2 * pi * results{di}.TOfRhoAndAngle.Rho_Midpoints * rhodelta;
        anglenorm = 2 * pi * sin(results{di}.TOfRhoAndAngle.Angle_Midpoints) * angledelta;
        disp(['Total transmittance captured by TOfRhoAndAngle detector: ' num2str(sum(sum(results{di}.TOfRhoAndAngle.Mean.*repmat(anglenorm',[1,size(rhonorm,2)]).*repmat(rhonorm,[size(anglenorm,2),1]))))]);
    end    
    if isfield(results{di}, 'TOfXAndY') && show.TOfXAndY
        figname = sprintf('log10(%s)',results{di}.TOfXAndY.Name); figure; imagesc(results{di}.TOfXAndY.Y_Midpoints, results{di}.TOfXAndY.X_Midpoints, log10(results{di}.TOfXAndY.Mean)); colorbar; title(figname); set(gcf,'Name', figname); ylabel('Y [mm]'); xlabel('X [mm]');
        xynorm = (results{di}.TOfXAndY.X(2)-results{di}.TOfXAndY.X(1))*(results{di}.TOfXAndY.Y(2)-results{di}.TOfXAndY.Y(1));
        disp(['Total transmittance captured by TOfXAndY detector: ' num2str(sum(results{di}.TOfXAndY.Mean(:)*xynorm))]);
        % determine range of x, y midpoints that have non-zero data
        [r,c]=find(results{di}.TOfXAndY.Mean);
        disp(sprintf('TOfXAndY: x non-zero span [%d %d]',min(r),max(r))); disp(sprintf('TOfXAndY: y non-zero span [%d %d]',min(c),max(c)));
    end
    if isfield(results{di}, 'TOfXAndYAndTimeAndSubregion') && show.TOfXAndYAndTimeAndSubregion
        y0idx = floor(length(results{di}.TOfXAndYAndTimeAndSubregion.Y_Midpoints)/2);
        for i=2:results{di}.TOfXAndYAndTimeAndSubregion.NumberOfRegions-1 % exclude air above and below          
          figname = sprintf('log10(%s) region idx=%i',results{di}.TOfXAndYAndTimeAndSubregion.Name,i-1); figure; 
          imagesc(results{di}.TOfXAndYAndTimeAndSubregion.X_Midpoints, results{di}.TOfXAndYAndTimeAndSubregion.Time_Midpoints,...
            log10(squeeze(results{di}.TOfXAndYAndTimeAndSubregion.Mean(i,:,y0idx,:)))); 
          colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('x [mm]');
        end
        xdelta = results{di}.TOfXAndYAndTimeAndSubregion.X(2)-results{di}.TOfXAndYAndTimeAndSubregion.X(1);        
        ydelta = results{di}.TOfXAndYAndTimeAndSubregion.Y(2)-results{di}.TOfXAndYAndTimeAndSubregion.Y(1);
        timedelta = results{di}.TOfXAndYAndTimeAndSubregion.Time(2)-results{di}.TOfXAndYAndTimeAndSubregion.Time(1);
        % the following does not integrate to diffuse T
        disp(['Total transmittance captured by TOfXAndYAndTimeAndSubregion detector: ' num2str(sum(xdelta*ydelta*timedelta*results{di}.TOfXAndYAndTimeAndSubregion.Mean(:)))]);
        % but this does
        disp(['Total transmittance captured by TOfXAndYAndTimeAndSubregion detector - TOfXAndY: ' num2str(sum(xdelta*ydelta*results{di}.TOfXAndYAndTimeAndSubregion.TOfXAndY(:)))]);
    end
    if isfield(results{di}, 'TOfFx') && show.TOfFx
        figname = sprintf('log10(%s)',results{di}.TOfFx.Name); figure; plot(results{di}.TOfFx.Fx_Midpoints, abs(results{di}.TOfFx.Mean)); title(figname); set(gcf,'Name', figname); 
        xlabel('f_x [/mm]'); ylabel('T(f_x) [unitless]');
        disp(['Total transmittance captured by TOfFx detector: ' num2str(results{di}.TOfFx.Amplitude(1))]);
    end
    if isfield(results{di}, 'ATotal') && show.ATotal
        disp(['Total absorption captured by ATotal detector: ' num2str(results{di}.ATotal.Mean)]);
    end
    if isfield(results{di}, 'AOfRhoAndZ') && show.AOfRhoAndZ
        numzs = length(results{di}.AOfRhoAndZ.Z)-1;
        figname = sprintf('log10(%s)',results{di}.AOfRhoAndZ.Name); figure; imagesc(results{di}.AOfRhoAndZ.Rho_Midpoints, results{di}.AOfRhoAndZ.Z_Midpoints, log10(results{di}.AOfRhoAndZ.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]');xlabel('\rho mm]');
        rhodelta = results{di}.AOfRhoAndZ.Rho(2)-results{di}.AOfRhoAndZ.Rho(1);
        zdelta = results{di}.AOfRhoAndZ.Z(2)-results{di}.AOfRhoAndZ.Z(1);
        rhonorm = 2 * pi * results{di}.AOfRhoAndZ.Rho_Midpoints * rhodelta;
        disp(['Absorbed energy captured by AOfRhoAndZ detector: ' num2str(sum(sum(zdelta*results{di}.AOfRhoAndZ.Mean.*repmat(rhonorm,[numzs,1]))))]);
    end
    if isfield(results{di}, 'AOfXAndYAndZ') && show.AOfXAndYAndZ
        numY = length(results{di}.AOfXAndYAndZ.Y) - 1;
        center=floor(numY/2);
        for i=center+1:center+1  % 1:numY
            figname = sprintf('log10(%s) y=%5.3f mm',results{di}.AOfXAndYAndZ.Name,results{di}.AOfXAndYAndZ.Y_Midpoints(i)); figure; imagesc(results{di}.AOfXAndYAndZ.X_Midpoints, results{di}.AOfXAndYAndZ.Z_Midpoints, log10(squeeze(results{di}.AOfXAndYAndZ.Mean(:,i,:)))); 
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]');
            xyznorm = (results{di}.AOfXAndYAndZ.X(2)-results{di}.AOfXAndYAndZ.X(1))*(results{di}.AOfXAndYAndZ.Y(2)-results{di}.AOfXAndYAndZ.Y(1))*(results{di}.AOfXAndYAndZ.Z(2)-results{di}.AOfXAndYAndZ.Z(1));
            disp(['Absorbed Energy captured by AOfXAndYAndZ detector: ' num2str(sum(results{di}.AOfXAndYAndZ.Mean(:)*xyznorm))]);
        end
    end
    if isfield(results{di}, 'ATotalBoundingVolume') && show.ATotalBoundingVolume
        disp(['Total absorption captured by ATotalBoundingVolume detector: ' num2str(results{di}.ATotalBoundingVolume.Mean)]);
    end
    if isfield(results{di}, 'FluenceOfRhoAndZ') && show.FluenceOfRhoAndZ
        numzs = length(results{di}.FluenceOfRhoAndZ.Z)-1;
        figname = sprintf('log10(%s)',results{di}.FluenceOfRhoAndZ.Name); figure; imagesc(results{di}.FluenceOfRhoAndZ.Rho_Midpoints, results{di}.FluenceOfRhoAndZ.Z_Midpoints, log10(results{di}.FluenceOfRhoAndZ.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]');colormap(jet);
        rhodelta = results{di}.FluenceOfRhoAndZ.Rho(2)-results{di}.FluenceOfRhoAndZ.Rho(1);
        zdelta = results{di}.FluenceOfRhoAndZ.Z(2)-results{di}.FluenceOfRhoAndZ.Z(1);
        rhonorm = 2 * pi * results{di}.FluenceOfRhoAndZ.Rho_Midpoints * rhodelta;
        disp(['Fluence captured by FluenceOfRhoAndZ detector: ' num2str(sum(sum(zdelta*results{di}.FluenceOfRhoAndZ.Mean.*repmat(rhonorm,[numzs,1]))))]);
    end
    if isfield(results{di}, 'FluenceOfRhoAndZAndTime') && show.FluenceOfRhoAndZAndTime
        numtimes = length(results{di}.FluenceOfRhoAndZAndTime.Time)-1;
        numzs = length(results{di}.FluenceOfRhoAndZAndTime.Z)-1;
        for i=1:10:numtimes % do every 10 time bins
            figname = sprintf('log10(%s) time=%5.3f ns',results{di}.FluenceOfRhoAndZAndTime.Name,results{di}.FluenceOfRhoAndZAndTime.Time_Midpoints(i)); figure; imagesc(results{di}.FluenceOfRhoAndZAndTime.Rho_Midpoints, results{di}.FluenceOfRhoAndZAndTime.Z_Midpoints, log10(squeeze(results{di}.FluenceOfRhoAndZAndTime.Mean(i,:,:)))); colormap(jet);
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]'); 
        end
        rhodelta = results{di}.FluenceOfRhoAndZAndTime.Rho(2)-results{di}.FluenceOfRhoAndZAndTime.Rho(1);
        timedelta = results{di}.FluenceOfRhoAndZAndTime.Time(2)-results{di}.FluenceOfRhoAndZAndTime.Time(1);
        zdelta = results{di}.FluenceOfRhoAndZAndTime.Z(2)-results{di}.FluenceOfRhoAndZAndTime.Z(1);
        rhonorm = 2 * pi * results{di}.FluenceOfRhoAndZAndTime.Rho_Midpoints * rhodelta;
        rhomatrix = repmat(rhonorm',[1,numzs,numtimes]);
        disp(['Fluence captured by FluenceOfRhoAndZAndTime detector: ' num2str(sum(sum(sum(timedelta*zdelta*results{di}.FluenceOfRhoAndZAndTime.Mean.*permute(rhomatrix,[3,2,1])))))]);
    end
    if isfield(results{di}, 'FluenceOfRhoAndZAndOmega') && show.FluenceOfRhoAndZAndOmega
        numomegas = length(results{di}.FluenceOfRhoAndZAndOmega.Omega);
        numrhos = length(results{di}.FluenceOfRhoAndZAndOmega.Rho)-1;
        numzs = length(results{di}.FluenceOfRhoAndZAndOmega.Z)-1;
        for i=1:10:numomegas % do every 10 omegas
            figname = sprintf('log10(%s:amplitude) omega=%5.3f GHz',results{di}.FluenceOfRhoAndZAndOmega.Name,results{di}.FluenceOfRhoAndZAndOmega.Omega_Midpoints(i)); 
            figure; imagesc(results{di}.FluenceOfRhoAndZAndOmega.Rho_Midpoints, results{di}.FluenceOfRhoAndZAndOmega.Z_Midpoints, log10(squeeze(results{di}.FluenceOfRhoAndZAndOmega.Amplitude(i,:,:)))); 
            colormap(jet);
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]'); 
        end
        rhodelta = results{di}.FluenceOfRhoAndZAndOmega.Rho(2)-results{di}.FluenceOfRhoAndZAndOmega.Rho(1);
        zdelta = results{di}.FluenceOfRhoAndZAndOmega.Z(2)-results{di}.FluenceOfRhoAndZAndOmega.Z(1);
        rhonorm = 2 * pi * results{di}.FluenceOfRhoAndZAndOmega.Rho_Midpoints * rhodelta;
        rhomatrix = repmat(rhonorm',[1,numzs]); % calculate total fluence at single omega
        disp(sprintf('Fluence captured by FluenceOfRhoAndZAndOmega detector at omega=%5.3f GHz: %5.3f',...
            results{di}.FluenceOfRhoAndZAndOmega.Omega_Midpoints(1),sum(sum(zdelta*squeeze(results{di}.FluenceOfRhoAndZAndOmega.Amplitude(1,:,:)).*permute(rhomatrix,[2,1]))))); %#ok<*DSPS>
    end
    if isfield(results{di}, 'FluenceOfXAndYAndZ') && show.FluenceOfXAndYAndZ
        numY = length(results{di}.FluenceOfXAndYAndZ.Y) - 1;
        center = floor(numY/2);
        for i=center+1:center+1 % 1:numY
            figname = sprintf('log10(%s) y=%5.3f mm',results{di}.FluenceOfXAndYAndZ.Name,results{di}.FluenceOfXAndYAndZ.Y_Midpoints(i)); figure; imagesc(results{di}.FluenceOfXAndYAndZ.X_Midpoints, results{di}.FluenceOfXAndYAndZ.Z_Midpoints, log10(squeeze(results{di}.FluenceOfXAndYAndZ.Mean(:,i,:)))); colormap(jet);
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]');
            xyznorm = (results{di}.FluenceOfXAndYAndZ.X(2)-results{di}.FluenceOfXAndYAndZ.X(1))*(results{di}.FluenceOfXAndYAndZ.Y(2)-results{di}.FluenceOfXAndYAndZ.Y(1))*(results{di}.FluenceOfXAndYAndZ.Z(2)-results{di}.FluenceOfXAndYAndZ.Z(1));
            disp(['Fluence captured by FluenceOfXAndYAndZ detector: ' num2str(sum(results{di}.FluenceOfXAndYAndZ.Mean(:)*xyznorm))]);
        end
    end    
    if isfield(results{di}, 'FluenceOfXAndYAndZAndTime') && show.FluenceOfXAndYAndZAndTime
        numtimes = length(results{di}.FluenceOfXAndYAndZAndTime.Time)-1;
        numxs = length(results{di}.FluenceOfXAndYAndZAndTime.X)-1;
        numys = length(results{di}.FluenceOfXAndYAndZAndTime.Y)-1;
        numzs = length(results{di}.FluenceOfXAndYAndZAndTime.Z)-1;
        center = floor(numys/2)+1;
        for i=1:10:numtimes % do every 10 Times
            figname = sprintf('log10(%s) y=0 time=%5.3f ns',results{di}.FluenceOfXAndYAndZAndTime.Name,results{di}.FluenceOfXAndYAndZAndTime.Time_Midpoints(i)); 
            figure; imagesc(results{di}.FluenceOfXAndYAndZAndTime.X_Midpoints, results{di}.FluenceOfXAndYAndZAndTime.Z_Midpoints, log10(squeeze(results{di}.FluenceOfXAndYAndZAndTime.Mean(i,:,center,:)))); 
            colormap(jet);
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]'); 
        end
        xdelta = results{di}.FluenceOfXAndYAndZAndTime.X(2)-results{di}.FluenceOfXAndYAndZAndTime.X(1);
        ydelta = results{di}.FluenceOfXAndYAndZAndTime.Y(2)-results{di}.FluenceOfXAndYAndZAndTime.Y(1);
        zdelta = results{di}.FluenceOfXAndYAndZAndTime.Z(2)-results{di}.FluenceOfXAndYAndZAndTime.Z(1);
        voxnorm = xdelta * ydelta * zdelta;
        disp(sprintf('Fluence captured by FluenceOfXAndYAndZAndTime detector at Time=%5.3f ns: %5.3f',...
            results{di}.FluenceOfXAndYAndZAndTime.Time_Midpoints(1),sum(sum(sum(voxnorm*results{di}.FluenceOfXAndYAndZAndTime.Mean(1,:,:,:))))));
    end   
    if isfield(results{di}, 'FluenceOfXAndYAndZAndOmega') && show.FluenceOfXAndYAndZAndOmega
        numomegas = length(results{di}.FluenceOfXAndYAndZAndOmega.Omega);
        numxs = length(results{di}.FluenceOfXAndYAndZAndOmega.X)-1;
        numys = length(results{di}.FluenceOfXAndYAndZAndOmega.Y)-1;
        numzs = length(results{di}.FluenceOfXAndYAndZAndOmega.Z)-1;
        center = floor(numys/2)+1;
        for i=1:10:numomegas % do every 10 omegas
            figname = sprintf('log10(%s:amplitude) y=0 omega=%5.3f GHz',results{di}.FluenceOfXAndYAndZAndOmega.Name,results{di}.FluenceOfXAndYAndZAndOmega.Omega_Midpoints(i)); 
            figure; imagesc(results{di}.FluenceOfXAndYAndZAndOmega.X_Midpoints, results{di}.FluenceOfXAndYAndZAndOmega.Z_Midpoints, log10(squeeze(results{di}.FluenceOfXAndYAndZAndOmega.Amplitude(i,:,center,:)))); 
            colormap(jet);
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]'); 
        end
        xdelta = results{di}.FluenceOfXAndYAndZAndOmega.X(2)-results{di}.FluenceOfXAndYAndZAndOmega.X(1);
        ydelta = results{di}.FluenceOfXAndYAndZAndOmega.Y(2)-results{di}.FluenceOfXAndYAndZAndOmega.Y(1);
        zdelta = results{di}.FluenceOfXAndYAndZAndOmega.Z(2)-results{di}.FluenceOfXAndYAndZAndOmega.Z(1);
        voxnorm = xdelta * ydelta * zdelta;
        disp(sprintf('Fluence captured by FluenceOfXAndYAndZAndOmega detector at omega=%5.3f GHz: %5.3f',...
            results{di}.FluenceOfXAndYAndZAndOmega.Omega_Midpoints(1),sum(sum(sum(voxnorm*results{di}.FluenceOfXAndYAndZAndOmega.Amplitude(1,:,:,:))))));
    end
    if isfield(results{di}, 'FluenceOfXAndYAndZAndStartingXAndY') && show.FluenceOfXAndYAndZAndStartingXAndY
        numxs = length(results{di}.FluenceOfXAndYAndZAndStartingXAndY.X)-1;
        numys = length(results{di}.FluenceOfXAndYAndZAndStartingXAndY.Y)-1;
        numzs = length(results{di}.FluenceOfXAndYAndZAndStartingXAndY.Z)-1;
        numsxs = length(results{di}.FluenceOfXAndYAndZAndStartingXAndY.StartingX)-1;        
        numsys = length(results{di}.FluenceOfXAndYAndZAndStartingXAndY.StartingY)-1;
        center = floor(numys/2)+1;
        % do for 1st Starting X=1,Y=1
        figname = sprintf('log10(Fluence(X,Y,Z)) StartingX=%5.3f StartingY=%5.3f',...
            results{di}.FluenceOfXAndYAndZAndStartingXAndY.StartingX_Midpoints(1),results{di}.FluenceOfXAndYAndZAndStartingXAndY.StartingY_Midpoints(1)); 
        figure; imagesc(results{di}.FluenceOfXAndYAndZAndStartingXAndY.X_Midpoints, results{di}.FluenceOfXAndYAndZAndStartingXAndY.Z_Midpoints, ...
            log10(squeeze(results{di}.FluenceOfXAndYAndZAndStartingXAndY.Mean(:,center,:,1,1)))); 
        colormap(jet);
        colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]');
        disp(sprintf('# photons starting in X=3, Y=1: %d',results{di}.FluenceOfXAndYAndZAndStartingXAndY.StartingXYCount(1,1)));
        xdelta = results{di}.FluenceOfXAndYAndZAndStartingXAndY.X(2)-results{di}.FluenceOfXAndYAndZAndStartingXAndY.X(1);
        ydelta = results{di}.FluenceOfXAndYAndZAndStartingXAndY.Y(2)-results{di}.FluenceOfXAndYAndZAndStartingXAndY.Y(1);
        zdelta = results{di}.FluenceOfXAndYAndZAndStartingXAndY.Z(2)-results{di}.FluenceOfXAndYAndZAndStartingXAndY.Z(1);
        voxnorm = xdelta * ydelta * zdelta;
        disp(sprintf('Fluence captured by FluenceOfXAndYAndZAndStartingXAndY: %5.3f',sum(voxnorm*results{di}.FluenceOfXAndYAndZAndStartingXAndY.Mean(:))));
    end
    if isfield(results{di}, 'FluenceOfFxAndZ') && show.FluenceOfFxAndZ
        numfxs = length(results{di}.FluenceOfFxAndZ.Fx);
        numzs = length(results{di}.FluenceOfFxAndZ.Z)-1;
        figname = sprintf('log10(%s:amplitude)',results{di}.FluenceOfFxAndZ.Name); 
        figure; imagesc(results{di}.FluenceOfFxAndZ.Fx_Midpoints, results{di}.FluenceOfFxAndZ.Z_Midpoints, log10(results{di}.FluenceOfFxAndZ.Amplitude)); 
        colormap(jet);
        colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('fx [/mm]');   
        zdelta = results{di}.FluenceOfFxAndZ.Z(2)-results{di}.FluenceOfFxAndZ.Z(1);
        disp(sprintf('Fluence captured by FluenceOfFxAndZ detector at fx=0: %5.3f',...
            sum(zdelta*results{di}.FluenceOfFxAndZ.Amplitude(:,1))));
    end
    if isfield(results{di}, 'RadianceOfRhoAndZAndAngle') && show.RadianceOfRhoAndZAndAngle
        numrhos = length(results{di}.RadianceOfRhoAndZAndAngle.Rho) - 1;
        numangles = length(results{di}.RadianceOfRhoAndZAndAngle.Angle) - 1;
        numzs = length(results{di}.RadianceOfRhoAndZAndAngle.Z) - 1;
        % create colorbar based on max, min values 
        minRadiance = min(results{di}.RadianceOfRhoAndZAndAngle.Mean(:));
        if minRadiance==0 % make sure don't take log of 0
            minRadiance=1e-5;
        end
        maxRadiance = max(results{di}.RadianceOfRhoAndZAndAngle.Mean(:));
        for i=1:numangles
            figname = sprintf('log10(%s) %5.3f<angle<%5.3f',results{di}.RadianceOfRhoAndZAndAngle.Name,(i-1)*pi/numangles,i*pi/numangles); 
            figure; imagesc(results{di}.RadianceOfRhoAndZAndAngle.Rho_Midpoints, results{di}.RadianceOfRhoAndZAndAngle.Z_Midpoints, log10(squeeze(results{di}.RadianceOfRhoAndZAndAngle.Mean(i,:,:)))); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]'); colormap(jet);
            caxis([log10(minRadiance),log10(maxRadiance)]);
        end
        % plot diff if two hemispheres
        if (numangles==2)
            figname = 'log10(lower./upper)'; 
            figure; imagesc(results{di}.RadianceOfRhoAndZAndAngle.Rho_Midpoints, results{di}.RadianceOfRhoAndZAndAngle.Z_Midpoints, ...
                squeeze(log10(results{di}.RadianceOfRhoAndZAndAngle.Mean(1,:,:)./results{di}.RadianceOfRhoAndZAndAngle.Mean(2,:,:)))); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('\rho [mm]');
        end
        rhodelta = results{di}.RadianceOfRhoAndZAndAngle.Rho(2)-results{di}.RadianceOfRhoAndZAndAngle.Rho(1);
        zdelta = results{di}.RadianceOfRhoAndZAndAngle.Z(2)-results{di}.RadianceOfRhoAndZAndAngle.Z(1);
        angledelta = results{di}.RadianceOfRhoAndZAndAngle.Angle(2)-results{di}.RadianceOfRhoAndZAndAngle.Angle(1);
        rhonorm = 2 * pi * results{di}.RadianceOfRhoAndZAndAngle.Rho_Midpoints * rhodelta;
        anglenorm = 2 * pi * sin(results{di}.RadianceOfRhoAndZAndAngle.Angle_Midpoints) * angledelta;
        rhomatrix = repmat(rhonorm',[1,numzs,numangles]);
        anglematrix = repmat(anglenorm',[1,numzs,numrhos]);
        disp(['Radiance captured by RadianceOfRhoAndZAndAngle detector: ' num2str(sum(sum(sum(zdelta*results{di}.RadianceOfRhoAndZAndAngle.Mean.*anglematrix.*permute(rhomatrix,[3,2,1])))))]);
    end
    if isfield(results{di}, 'RadianceOfFxAndZAndAngle') && show.RadianceOfFxAndZAndAngle
        numfxs = length(results{di}.RadianceOfFxAndZAndAngle.Fx);
        numangles = length(results{di}.RadianceOfFxAndZAndAngle.Angle) - 1;
        numzs = length(results{di}.RadianceOfFxAndZAndAngle.Z) - 1;
        % create colorbar based on max, min values 
        minRadiance = min(abs(results{di}.RadianceOfFxAndZAndAngle.Mean(:)));
        if minRadiance==0 % make sure don't take log of 0
            minRadiance=1e-5;
        end
        maxRadiance = max(abs(results{di}.RadianceOfFxAndZAndAngle.Mean(:)));
        for i=1:numangles
            figname = sprintf('log10(%s) amplitude %5.3f<angle<%5.3f',results{di}.RadianceOfFxAndZAndAngle.Name,(i-1)*pi/numangles,i*pi/numangles); 
            figure; imagesc(results{di}.RadianceOfFxAndZAndAngle.Fx_Midpoints, results{di}.RadianceOfFxAndZAndAngle.Z_Midpoints, log10(squeeze(results{di}.RadianceOfFxAndZAndAngle.Amplitude(i,:,:)))); colormap(jet);
            colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('fx [/mm]');
            %caxis([log10(minRadiance),log10(maxRadiance)]);
            % plot line scan of radiance at select fxs
            figure;
            k=1;
            for j=1:10:51
                plot(results{di}.RadianceOfFxAndZAndAngle.Z_Midpoints(1:end-1), log10(results{di}.RadianceOfFxAndZAndAngle.Amplitude(i,1:end-1,j)));
                title(sprintf('%5.3f<angle<%5.3f',(i-1)*pi/numangles,i*pi/numangles));
                ylabel('log Radiance amplitude');xlabel('z [mm]');
                hold on;
                ar{k}=sprintf('f_x = %s',num2str(results{di}.RadianceOfFxAndZAndAngle.Fx_Midpoints(j)));
                k=k+1;
                colormap(jet);
            end
            legend(ar);
%             % plot relative error
%             figure; imagesc(results{di}.RadianceOfFxAndZAndAngle.Fx_Midpoints, results{di}.RadianceOfFxAndZAndAngle.Z_Midpoints, ...
%             (squeeze(abs(results{di}.RadianceOfFxAndZAndAngle.Stdev(i,:,:))./results{di}.RadianceOfFxAndZAndAngle.Amplitude(i,:,:))));
%             colorbar; caxis([0 1]);title(sprintf('Relative Error Amplitude %5.3f<angle<%5.3f',(i-1)*pi/numangles,i*pi/numangles));ylabel('z [mm]');xlabel('fx [/mm]');
%             % plot line scan of relative error at select fxs
%             f=figure;
%             k=1;
%             for j=1:10:51
%                 plot(results{di}.RadianceOfFxAndZAndAngle.Z_Midpoints(1:end-1), ...
%                     results{di}.RadianceOfFxAndZAndAngle.Stdev(i,1:end-1,j)./results{di}.RadianceOfFxAndZAndAngle.Amplitude(i,1:end-1,j));
%                 title(sprintf('%5.3f<angle<%5.3f',(i-1)*pi/numangles,i*pi/numangles));
%                 ylabel('Relative Error');xlabel('z [mm]'); axis([0 results{di}.RadianceOfFxAndZAndAngle.Z(end), 0 0.4]);
%                 hold on;
%                 ar{k}=sprintf('f_x = %s',num2str(results{di}.RadianceOfFxAndZAndAngle.Fx_Midpoints(j)));
%                 k=k+1;
%             end
%             legend(ar);
%             line([0 10.0],[0.05 0.05],'Color',[0 0 0],'LineStyle',':');
        end
        zdelta = results{di}.RadianceOfFxAndZAndAngle.Z(2)-results{di}.RadianceOfFxAndZAndAngle.Z(1);
        angledelta = results{di}.RadianceOfFxAndZAndAngle.Angle(2)-results{di}.RadianceOfFxAndZAndAngle.Angle(1);
        anglenorm = 2 * pi * sin(results{di}.RadianceOfFxAndZAndAngle.Angle_Midpoints) * angledelta; 
        anglematrix = repmat(anglenorm',[1,numzs]);
        disp(['Radiance captured by RadianceOfFxAndZAndAngle detector for fx=0: ' num2str(sum(sum(sum(zdelta*results{di}.RadianceOfFxAndZAndAngle.Amplitude(:,:,1).*anglematrix))))]);
    end
    if isfield(results{di}, 'RadianceOfXAndYAndZAndThetaAndPhi') && show.RadianceOfXAndYAndZAndThetaAndPhi
        numxs = length(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X) - 1;
        numys = length(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Y) - 1;
        numzs = length(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z) - 1;
        numphis = length(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Phi) - 1;
        numthetas = length(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta) - 1;      
        % plot radiance vs x and z for each theta (polar angle from Uz=[-1:1]
        for i=1:numthetas % note results array has dimensions [numphis, numthetas, numzs, numys, numxs] due to column major json reading
            figname = sprintf('log10(%s) %5.3f<Theta<%5.3f',results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Name,(i-1)*pi/numthetas,i*pi/numthetas); 
            figure; imagesc(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X_Midpoints, results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z_Midpoints, log10(squeeze(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Mean(1,i,:,1,:))), [-20 -5]); colorbar; title(figname); set(gcf,'Name', figname);ylabel('z [mm]'); xlabel('x [mm]');colormap(jet);
        end
        xyzphinorm = (results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.X(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Y(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Y(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Z(1))...
                         *(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Phi(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Phi(1)); 
        partialsum=xyzphinorm*sum(sum(sum(sum(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Mean,1),3),4),5);
        thetadelta = results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta(2)-results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta(1);
        thetanorm = sin(results{di}.RadianceOfXAndYAndZAndThetaAndPhi.Theta_Midpoints) * thetadelta;  
        disp(['Radiance captured by RadianceOfXAndYAndZAndThetaAndPhi detector: ' num2str(sum(partialsum.*thetanorm))]);
    end
    if isfield(results{di}, 'ReflectedMTOfRhoAndSubregionHist') && show.ReflectedMTOfRhoAndSubregionHist
        numrhos = length(results{di}.ReflectedMTOfRhoAndSubregionHist.Rho) - 1;
        numsubregions = length(results{di}.ReflectedMTOfRhoAndSubregionHist.SubregionIndices)
        figname = sprintf('log10(%s)',results{di}.ReflectedMTOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.ReflectedMTOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints, log10(results{di}.ReflectedMTOfRhoAndSubregionHist.Mean));...        
           colorbar; title(figname); xlabel('\rho [mm]'); ylabel('MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numSubregions, numMTBins, numRhos] due to column major json reading
        for j=2:3 % customized, general form: j=1:numsubregions
        for i=1:20:numrhos
            %figure; plot(results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints,results{di}.ReflectedMTOfRhoAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Reflected Fractional MT in Region %2d, Rho = %5.3f mm',j-1,results{di}.ReflectedMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            MT=results{di}.ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(:,j,:,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('MT'),ylabel('photon weight');
%           stack=zeros(size(results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(1,j,:,i)));
%             for k=1:size(results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT,1)                
%                 %stack=stack+results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(i,:,j,k);
%                 stack=stack+results{di}.ReflectedMTOfRhoAndSubregionHist.FractionalMT(k,j,:,i);
%                 semilogy(X,squeeze(stack),color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('MT'),ylabel('stacked log10(photon weight)'); hold on;
%             end
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar);   
        end
        end
    end
    if isfield(results{di}, 'ReflectedMTOfXAndYAndSubregionHist') && show.ReflectedMTOfXAndYAndSubregionHist
        numxs = length(results{di}.ReflectedMTOfXAndYAndSubregionHist.X) - 1;
        numys = length(results{di}.ReflectedMTOfXAndYAndSubregionHist.Y) - 1;
        numsubregions = length(results{di}.ReflectedMTOfXAndYAndSubregionHist.SubregionIndices);
        figname = sprintf('log10(%s) summed over y',results{di}.ReflectedMTOfXAndYAndSubregionHist.Name); 
        % plot results summed over y indices
        figure; imagesc(results{di}.ReflectedMTOfXAndYAndSubregionHist.X_Midpoints, results{di}.ReflectedMTOfXAndYAndSubregionHist.MTBins_Midpoints, log10(squeeze(sum(results{di}.ReflectedMTOfXAndYAndSubregionHist.Mean,2))));...        
           colorbar; title(figname); xlabel('x [mm]'); ylabel('MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numSubregions, numMTBins, numRhos] due to column major json reading
        for j=2:3 % customized, general form: j=1:numsubregions
        center=floor(numxs/2);
        for i=center:center+1 % customized for just those x near source, general form: i=1:numxs for every x bin
            %figure; plot(results{di}.ReflectedMTOfXAndYAndSubregionHist.MTBins_Midpoints,results{di}.ReflectedMTOfXAndYAndSubregionHist.Mean(i,1,:)); % debug plots
            figure;figname = sprintf('Reflected Fractional MT in Region %2d, X = %5.3f mm',j-1,results{di}.ReflectedMTOfXAndYAndSubregionHist.X_Midpoints(i));
            MT=results{di}.ReflectedMTOfXAndYAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.ReflectedMTOfXAndYAndSubregionHist.FractionalMT(:,j,:,1,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('MT'),ylabel('photon weight');
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar);  
        end
        end
    end
    if isfield(results{di}, 'TransmittedMTOfRhoAndSubregionHist') && show.TransmittedMTOfRhoAndSubregionHist
        numrhos = length(results{di}.TransmittedMTOfRhoAndSubregionHist.Rho) - 1;
        numsubregions = length(results{di}.TransmittedMTOfRhoAndSubregionHist.SubregionIndices);
        figname = sprintf('log10(%s)',results{di}.TransmittedMTOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.TransmittedMTOfRhoAndSubregionHist.Rho_Midpoints, results{di}.TransmittedMTOfRhoAndSubregionHist.MTBins_Midpoints, log10(results{di}.TransmittedMTOfRhoAndSubregionHist.Mean));...        
           colorbar; title(figname); xlabel('\rho [mm]'); ylabel('MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numSubregions, numMTBins, numRhos] due to column major json reading
        for j=2:3 % customized, general form: j=1:numsubregions
        for i=1:20:numrhos
            %figure; plot(results{di}.TransmittedMTOfRhoAndSubregionHist.MTBins_Midpoints,results{di}.TransmittedMTOfRhoAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Transmitted Fractional MT in Region %2d, Rho = %5.3f mm',j-1,results{di}.TransmittedMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            MT=results{di}.TransmittedMTOfRhoAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.TransmittedMTOfRhoAndSubregionHist.FractionalMT(:,j,:,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('MT'),ylabel('photon weight');
%           stack=zeros(size(results{di}.TransmittedMTOfRhoAndSubregionHist.FractionalMT(1,j,:,i)));
%             for k=1:size(results{di}.TransmittedMTOfRhoAndSubregionHist.FractionalMT,1)                
%                 %stack=stack+results{di}.TransmittedMTOfRhoAndSubregionHist.FractionalMT(i,:,j,k);
%                 stack=stack+results{di}.TransmittedMTOfRhoAndSubregionHist.FractionalMT(k,j,:,i);
%                 semilogy(X,squeeze(stack),color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('MT'),ylabel('stacked log10(photon weight)'); hold on;
%             end
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar);
        end
        end
    end
    if isfield(results{di}, 'TransmittedMTOfXAndYAndSubregionHist') && show.TransmittedMTOfXAndYAndSubregionHist
        numxs = length(results{di}.TransmittedMTOfXAndYAndSubregionHist.X) - 1;
        numys = length(results{di}.TransmittedMTOfXAndYAndSubregionHist.Y) - 1;
        numsubregions = length(results{di}.TransmittedMTOfXAndYAndSubregionHist.SubregionIndices);
        figname = sprintf('log10(%s) at y=0',results{di}.TransmittedMTOfXAndYAndSubregionHist.Name); 
        % plot results summed over y indices
        figure; imagesc(results{di}.TransmittedMTOfXAndYAndSubregionHist.X_Midpoints, results{di}.TransmittedMTOfXAndYAndSubregionHist.MTBins_Midpoints, log10(squeeze(sum(results{di}.TransmittedMTOfXAndYAndSubregionHist.Mean,2))));...        
           colorbar; title(figname); xlabel('x [mm]'); ylabel('MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numSubregions, numMTBins, numRhos] due to column major json reading
        for j=2:3 % customized, general form: j=1:numsubregions
        center=floor(numxs/2);
        for i=center:center+1 % customized, general form: i=1:numxs
            %figure; plot(results{di}.TransmittedMTOfXAndYAndSubregionHist.MTBins_Midpoints,results{di}.TransmittedMTOfXAndYAndSubregionHist.Mean(i,1,:)); % debug plots
            figure;figname = sprintf('Reflected Fractional MT in Region %2d, X = %5.3f mm',j-1,results{di}.TransmittedMTOfXAndYAndSubregionHist.X_Midpoints(i));
            MT=results{di}.TransmittedMTOfXAndYAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.TransmittedMTOfXAndYAndSubregionHist.FractionalMT(:,j,:,1,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('MT'),ylabel('photon weight');
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar); 
        end
        end
    end
    if isfield(results{di}, 'ReflectedDynamicMTOfRhoAndSubregionHist') && show.ReflectedDynamicMTOfRhoAndSubregionHist
        numrhos = length(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Rho) - 1;
        figname = sprintf('log10(%s)',results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints, log10(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Mean));...        
           colorbar; title(figname); xlabel('\rho [mm]'); ylabel('Dynamic MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numMTBins, numRhos] due to column major json reading
        for i=1:20:numrhos
            %figure; plot(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints,results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Reflected Fractional Dynamic MT, Rho = %5.3f mm',results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            MT=results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT(:,:,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('Dynamic MT'),ylabel('photon weight');
%           stack=zeros(size(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT(1,j,:,i)));
%             for k=1:size(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT,1)                
%                 %stack=stack+results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT(i,:,j,k);
%                 stack=stack+results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT(k,j,:,i);
%                 semilogy(X,squeeze(stack),color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('MT'),ylabel('stacked log10(photon weight)'); hold on;
%             end
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar);
            figure;figname = sprintf('Reflected Total MT of Z, Rho = %5.3f mm',results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            errorbar(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Z_Midpoints,results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ(:,i),...
                results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZStdev(:,i));
            title(figname);xlabel('z (mm)');ylabel('Total MT');
            figure;figname = sprintf('Reflected Dynamic MT of Z, Rho = %5.3f mm',results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            errorbar(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.Z_Midpoints,results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ(:,i),...
                results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZStdev(:,i));
            title(figname);xlabel('z (mm)');ylabel('Dynamic MT');
        end
        figure;figname='Reflected Dynamic MT Of Rho: Subregion Collisions';
        plot(results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.SubregionIndices, results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.SubregionCollisions(1,:),...
             results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.SubregionIndices, results{di}.ReflectedDynamicMTOfRhoAndSubregionHist.SubregionCollisions(2,:));
        title(figname);xlabel('tissue region index');ylabel('Collisions');legend('static','dynamic');
    end
    if isfield(results{di}, 'ReflectedDynamicMTOfXAndYAndSubregionHist') && show.ReflectedDynamicMTOfXAndYAndSubregionHist
        numxs = length(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.X) - 1;
        numys = length(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Y) - 1;
        figname = sprintf('log10(%s) summed over y',results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Name); 
        % plot results summed over y indices
        figure; imagesc(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.X_Midpoints, results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints, log10(squeeze(sum(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Mean,2))));...        
           colorbar; title(figname); xlabel('x [mm]'); ylabel('Dynamic MT'); set(gcf,'Name', figname);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numMTBins,numYs,numXs] due to column major json reading
        xcenter=floor(numxs/2);
        ycenter=floor(numys/2);
        for i=xcenter:xcenter+1 % customized for just those x near source, general form: i=1:numxs for every x bin
            %figure; plot(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints,results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Mean(i,1,:)); % debug plots
            figure;figname = sprintf('Reflected Fractional Dynamic MT, X = %5.3f mm, Y = %5.3f mm',results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.X_Midpoints(i),...
                results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints(ycenter));
            MT=results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.FractionalMT(:,:,1,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('MT'),ylabel('photon weight');
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar); 
            figure;figname = sprintf('Reflected Total MT of Z, X = %5.3f mm, Y = %5.3f mm',results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.X_Midpoints(i),...
                results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints(ycenter));
            plot(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints,results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ(:,1,i));
            title(figname);xlabel('z (mm)');ylabel('Total MT');
            figure;figname = sprintf('Reflected Dynamic MT of Z, X = %5.3f mm, Y = %5.3f mm',results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.X_Midpoints(i),...
                results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints(ycenter));
            plot(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints,results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ(:,1,i));
            title(figname);xlabel('z (mm)');ylabel('Dynamic MT');
        end
        figure;figname='Reflected Dynamic MT Of X and Y: Subregion Collisions';
        plot(results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionIndices, results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions(1,:),...
             results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionIndices, results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions(2,:));
        title(figname);xlabel('tissue region index');ylabel('Collisions');legend('static','dynamic');
    end
    if isfield(results{di}, 'ReflectedDynamicMTOfFxAndSubregionHist') && show.ReflectedDynamicMTOfFxAndSubregionHist
        numFxs = length(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Fx);
        figname = sprintf('log10(%s)',results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Name); 
        figure; 
        imagesc(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Fx_Midpoints, results{di}.ReflectedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints, ...
            log10(abs(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Mean)));       
        colorbar; title(figname); xlabel('Fx [/mm]'); ylabel('Dynamic MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numMTBins, numFxs] due to column major json reading
        for i=1:10:numFxs
            %figure; plot(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints,results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Reflected Fractional Dynamic MT, Fx = %5.3f mm',results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Fx_Midpoints(i));
            MT=results{di}.ReflectedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(abs(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT(:,:,i)));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('Dynamic MT'),ylabel('photon weight');
%           stack=zeros(size(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT(1,j,:,i)));
%             for k=1:size(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT,1)                
%                 %stack=stack+results{di}.ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT(i,:,j,k);
%                 stack=stack+results{di}.ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT(k,j,:,i);
%                 semilogy(X,squeeze(stack),color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('MT'),ylabel('stacked log10(photon weight)'); hold on;
%             end
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar);
            figure;figname = sprintf('Reflected Total MT of Z, Fx = %5.3f mm',results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Fx_Midpoints(i));
            errorbar(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Z_Midpoints,abs(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZ(:,i)),...
                abs(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZStdev(:,i)));
            title(figname);xlabel('z (mm)');ylabel('Total MT');
            figure;figname = sprintf('Reflected Dynamic MT of Z, Fx = %5.3f mm',results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Fx_Midpoints(i));
            errorbar(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.Z_Midpoints,abs(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ(:,i)),...
                abs(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZStdev(:,i)));
            title(figname);xlabel('z (mm)');ylabel('Dynamic MT');
        end
        figure;figname='Reflected Dynamic MT Of Fx: Subregion Collisions';
        plot(results{di}.ReflectedDynamicMTOfFxAndSubregionHist.SubregionIndices, results{di}.ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions(1,:),...
             results{di}.ReflectedDynamicMTOfFxAndSubregionHist.SubregionIndices, results{di}.ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions(2,:));
        title(figname);xlabel('tissue region index');ylabel('Collisions');legend('static','dynamic');
    end
    if isfield(results{di}, 'TransmittedDynamicMTOfRhoAndSubregionHist') && show.TransmittedDynamicMTOfRhoAndSubregionHist
        numrhos = length(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Rho) - 1;
        figname = sprintf('log10(%s)',results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints, results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints, log10(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Mean));...        
           colorbar; title(figname); xlabel('\rho [mm]'); ylabel('Dynamic MT'); set(gcf,'Name', figname);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numMTBins, numRhos] due to column major json reading
        for i=1:20:41 % customized, general form: i=1:numrhos
            %figure; plot(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints,results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Transmitted Fractional Dynamic MT, Rho = %5.3f mm',results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            MT=results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT(:,:,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('Dynamic MT'),ylabel('photon weight');
%           stack=zeros(size(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT(1,j,:,i)));
%             for k=1:size(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT,1)                
%                 %stack=stack+results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT(i,:,j,k);
%                 stack=stack+results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT(k,j,:,i);
%                 semilogy(X,squeeze(stack),color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('DynamicMT'),ylabel('stacked log10(photon weight)'); hold on;
%             end
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar); 
            figure;figname = sprintf('Transmitted Total MT of Z, Rho = %5.3f mm',results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints(i));      
            plot(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Z_Midpoints,results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ(:,i));
            title(figname);xlabel('z (mm)');ylabel('Total MT');
            figure;figname = sprintf('Transmitted Dynamic MT of Z, Rho = %5.3f mm',results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints(i));
            plot(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.Z_Midpoints,results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ(:,i));
            title(figname);xlabel('z (mm)');ylabel('Dynamic MT');
        end
        figure;figname='Transmitted Dynamic MT Of Rho: Subregion Collisions';
        plot(results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.SubregionIndices, results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.SubregionCollisions(1,:),...
             results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.SubregionIndices, results{di}.TransmittedDynamicMTOfRhoAndSubregionHist.SubregionCollisions(2,:));
        title(figname);xlabel('tissue region index');ylabel('Collisions');legend('static','dynamic');
    end
    if isfield(results{di}, 'TransmittedDynamicMTOfXAndYAndSubregionHist') && show.TransmittedDynamicMTOfXAndYAndSubregionHist
        numxs = length(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.X) - 1;
        numys = length(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Y) - 1;
        figname = sprintf('log10(%s) at y=0',results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Name); 
        % plot results summed over y indices
        figure; imagesc(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.X_Midpoints, results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints, log10(squeeze(sum(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Mean,2))));...        
           colorbar; title(figname); xlabel('x [mm]'); ylabel('Dynamic MT'); set(gcf,'Name', figname);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numMTBins,numYs,numXs] due to column major json reading
        xcenter=floor(numxs/2);
        ycenter=floor(numys/2);
        for i=xcenter:xcenter+1 % customized, general form: i=1:numxs
            %figure; plot(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints,results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Mean(i,1,:)); % debug plots
            figure;figname = sprintf('Transmitted Fractional Dynamic MT, X = %5.3f mm, Y = %5.3f mm',results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.X_Midpoints(i),...
                results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints(ycenter));
            MT=results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT(:,:,ycenter,i));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('Dynamic MT'),ylabel('photon weight');
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar); 
            figure;figname = sprintf('Transmitted Total MT of Z, X = %5.3f mm, Y = %5.3f mm',results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.X_Midpoints(i),...
                results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints(ycenter));
            plot(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints,results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ(:,ycenter,i));
            title(figname);xlabel('z (mm)');ylabel('Total MT');
            figure;figname = sprintf('Transmitted Dynamic MT of Z, X = %5.3f mm, Y = %5.3f mm',results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.X_Midpoints(i),...
                results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints(ycenter));
            plot(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints,results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ(:,ycenter,i));
            title(figname);xlabel('z (mm)');ylabel('Dynamic MT');
        end
        figure;figname='Transmitted Dynamic MT Of X and Y: Subregion Collisions';
        plot(results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionIndices, results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions(1,:),...
             results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionIndices, results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions(2,:));
        title(figname);xlabel('tissue region index');ylabel('Collisions');legend('static','dynamic');
    end   
    if isfield(results{di}, 'TransmittedDynamicMTOfFxAndSubregionHist') && show.TransmittedDynamicMTOfFxAndSubregionHist
        numFxs = length(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Fx);
        figname = sprintf('log10(%s)',results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Name); 
        figure; 
        imagesc(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Fx_Midpoints, results{di}.TransmittedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints,...
            log10(abs(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Mean)));...        
        colorbar; title(figname); xlabel('Fx [/mm]'); ylabel('Dynamic MT'); set(gcf,'Name', figname);colormap(jet);
        color=char('r-','g-','b-','c-','m-','r:','g:','b:','c:','m:');
        % note results array has dimensions [numFractionalMTBins,numMTBins, numFxs] due to column major json reading
        for i=1:10:numFxs
            %figure; plot(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints,results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Mean(i,:)); % debug plots
            figure;figname = sprintf('Transmitted Fractional Dynamic MT, Fx = %5.3f mm',results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Fx_Midpoints(i));
            MT=results{di}.TransmittedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints;
            layerfrac=squeeze(abs(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT(:,:,i)));
            bar(MT,layerfrac','stacked'); title(figname);xlabel('Dynamic MT'),ylabel('photon weight');
%           stack=zeros(size(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT(1,j,:,i)));
%             for k=1:size(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT,1)                
%                 %stack=stack+results{di}.TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT(i,:,j,k);
%                 stack=stack+results{di}.TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT(k,j,:,i);
%                 semilogy(X,squeeze(stack),color(k,:),'LineWidth',3);axis([0 max(X) 1e-7 1]);title(figname);xlabel('MT'),ylabel('stacked log10(photon weight)'); hold on;
%             end
            % variable size legend based on layerfrac size
            numfracs=size(layerfrac,1);
            ar{1}='=0';ar{numfracs}='=1';
            for k=2:numfracs-1
                ar{k}=sprintf('[%3.2f-%3.2f]',(1.0/(numfracs-2))*(k-2),(1.0/(numfracs-2))*(k-1));
            end
            legend(ar);
            figure;figname = sprintf('Transmitted Total MT of Z, Fx = %5.3f mm',results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Fx_Midpoints(i));
            errorbar(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Z_Midpoints,abs(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZ(:,i)),...
                abs(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZStdev(:,i)));
            title(figname);xlabel('z (mm)');ylabel('Total MT');
            figure;figname = sprintf('Transmitted Dynamic MT of Z, Fx = %5.3f mm',results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Fx_Midpoints(i));
            errorbar(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.Z_Midpoints,results{di}.TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ(:,i),...
                results{di}.TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZStdev(:,i));
            title(figname);xlabel('z (mm)');ylabel('Dynamic MT');
        end
        figure;figname='Transmitted Dynamic MT Of Fx: Subregion Collisions';
        plot(results{di}.TransmittedDynamicMTOfFxAndSubregionHist.SubregionIndices, results{di}.TransmittedDynamicMTOfFxAndSubregionHist.SubregionCollisions(1,:),...
             results{di}.TransmittedDynamicMTOfFxAndSubregionHist.SubregionIndices, results{di}.TransmittedDynamicMTOfFxAndSubregionHist.SubregionCollisions(2,:));
        title(figname);xlabel('tissue region index');ylabel('Collisions');legend('static','dynamic');
    end    
    if isfield(results{di}, 'ReflectedTimeOfRhoAndSubregionHist') && show.ReflectedTimeOfRhoAndSubregionHist
        numtissueregions = length(results{di}.ReflectedTimeOfRhoAndSubregionHist.SubregionIndices);
        for i=1:numtissueregions
            figname = sprintf('log10(%s) Region Index %d',results{di}.ReflectedTimeOfRhoAndSubregionHist.Name, i-1); 
            figure; imagesc(results{di}.ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedTimeOfRhoAndSubregionHist.Time_Midpoints, log10(squeeze(results{di}.ReflectedTimeOfRhoAndSubregionHist.Mean(:,i,:)')));       
               colorbar; caxis([-15 0]);title(figname); set(gcf,'Name', figname); ylabel('time [ns]'); xlabel('\rho [mm]');
        end
        figname = sprintf('%s Fractional Time',results{di}.ReflectedTimeOfRhoAndSubregionHist.Name); 
        figure; imagesc(results{di}.ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints, results{di}.ReflectedTimeOfRhoAndSubregionHist.Time_Midpoints, results{di}.ReflectedTimeOfRhoAndSubregionHist.FractionalTime');       
               colorbar; title(figname); set(gcf,'Name', figname); ylabel('subregion index'); xlabel('\rho [mm]')
        disp(['Time in Subregion captured by ReflectedTimeOfRhoAndSubregionHist detector: ' num2str(sum(results{di}.ReflectedTimeOfRhoAndSubregionHist.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCATotal') && show.pMCATotal
       disp(['Total absorption captured by pMCATotal detector: ' num2str(results{di}.pMCATotal.Mean)]);
    end
    if isfield(results{di}, 'pMCROfRho') && show.pMCROfRho
        figname = sprintf('log10(%s)',results{di}.pMCROfRho.Name); figure; plot(results{di}.pMCROfRho.Rho_Midpoints, log10(results{di}.pMCROfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('pMC R(\rho) [mm^-^2]');
        rhodelta = results{di}.pMCROfRho.Rho(2)-results{di}.pMCROfRho.Rho(1);
        rhonorm = 2 * pi * results{di}.pMCROfRho.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by pMCROfRho detector: ' num2str(sum(results{di}.pMCROfRho.Mean.*rhonorm'))]);
     end
    if isfield(results{di}, 'pMCROfRhoRecessed') && show.pMCROfRhoRecessed
        figname = sprintf('log10(%s)',results{di}.pMCROfRhoRecessed.Name); figure; plot(results{di}.pMCROfRhoRecessed.Rho_Midpoints, log10(results{di}.pMCROfRhoRecessed.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('pMC R(\rho) [mm^-^2]');
        rhodelta = results{di}.pMCROfRhoRecessed.Rho(2)-results{di}.pMCROfRhoRecessed.Rho(1);
        rhonorm = 2 * pi * results{di}.pMCROfRho.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by pMCROfRhoRecessed detector: ' num2str(sum(results{di}.pMCROfRhoRecessed.Mean.*rhonorm'))]);
     end
    if isfield(results{di}, 'pMCROfRhoAndTime') && show.pMCROfRhoAndTime
        figname = sprintf('log10(%s)',results{di}.pMCROfRhoAndTime.Name); figure; imagesc(results{di}.pMCROfRhoAndTime.Rho_Midpoints, results{di}.pMCROfRhoAndTime.Time_Midpoints,log10(results{di}.pMCROfRhoAndTime.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('\rho [mm]');
        numtimes = length(results{di}.pMCROfRhoAndTime.Time)-1;   
        timedelta = results{di}.pMCROfRhoAndTime.Time(2)-results{di}.pMCROfRhoAndTime.Time(1);
        rhodelta = results{di}.pMCROfRhoAndTime.Rho(2)-results{di}.pMCROfRhoAndTime.Rho(1);
        rhonorm = 2 * pi * results{di}.pMCROfRhoAndTime.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by pMCROfRhoAndTime detector: ' num2str(sum(sum(timedelta*results{di}.pMCROfRhoAndTime.Mean.*repmat(rhonorm,[numtimes,1]))))]);
     end   
    if isfield(results{di}, 'pMCROfRhoAndTimeRecessed') && show.pMCROfRhoAndTimeRecessed
        figname = sprintf('log10(%s)',results{di}.pMCROfRhoAndTimeRecessed.Name); figure; imagesc(results{di}.pMCROfRhoAndTimeRecessed.Rho_Midpoints, results{di}.pMCROfRhoAndTimeRecessed.Time_Midpoints,log10(results{di}.pMCROfRhoAndTimeRecessed.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('\rho [mm]');
        numtimes = length(results{di}.pMCROfRhoAndTimeRecessed.Time)-1;   
        timedelta = results{di}.pMCROfRhoAndTimeRecessed.Time(2)-results{di}.pMCROfRhoAndTimeRecessed.Time(1);
        rhodelta = results{di}.pMCROfRhoAndTimeRecessed.Rho(2)-results{di}.pMCROfRhoAndTimeRecessed.Rho(1);
        rhonorm = 2 * pi * results{di}.pMCROfRhoAndTimeRecessed.Rho_Midpoints * rhodelta;
        disp(['Total reflectance captured by ROfRhoAndTimeRecessed detector: ' num2str(sum(sum(timedelta*results{di}.pMCROfRhoAndTimeRecessed.Mean.*repmat(rhonorm,[numtimes,1]))))]);
    end 
    if isfield(results{di}, 'pMCROfXAndY') && show.pMCROfXAndY
        figname = sprintf('log10(%s)',results{di}.pMCROfXAndY.Name); figure; imagesc(results{di}.pMCROfXAndY.X_Midpoints, results{di}.pMCROfXAndY.Y_Midpoints,log10(results{di}.pMCROfXAndY.Mean)); colorbar; title(figname); set(gcf,'Name', figname);ylabel('y [mm]'); xlabel('x [mm]');
        xdelta = results{di}.ROfXAndYAndTime.X(2)-results{di}.ROfXAndYAndTime.X(1);        
        ydelta = results{di}.ROfXAndYAndTime.Y(2)-results{di}.ROfXAndYAndTime.Y(1);
        disp(['Total reflectance captured by pMCROfXAndY detector: ' num2str(sum(xdelta*ydelta*results{di}.pMCROfXAndY.Mean(:)))]);
    end
    if isfield(results{di}, 'pMCROfXAndYAndTimeAndSubregion') && show.pMCROfXAndYAndTimeAndSubregion
        y0idx = floor(length(results{di}.pMCROfXAndYAndTimeAndSubregion.Y_Midpoints)/2);
        for i=2:results{di}.pMCROfXAndYAndTimeAndSubregion.NumberOfRegions-1 % exclude air above and below          
          figname = sprintf('log10(%s) region idx=%i',results{di}.pMCROfXAndYAndTimeAndSubregion.Name,i-1); figure; 
          imagesc(results{di}.pMCROfXAndYAndTimeAndSubregion.X_Midpoints, results{di}.pMCROfXAndYAndTimeAndSubregion.Time_Midpoints,...
            log10(squeeze(results{di}.pMCROfXAndYAndTimeAndSubregion.Mean(i,:,y0idx,:)))); 
          colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('x [mm]');
        end
        xdelta = results{di}.pMCROfXAndYAndTimeAndSubregion.X(2)-results{di}.pMCROfXAndYAndTimeAndSubregion.X(1);        
        ydelta = results{di}.pMCROfXAndYAndTimeAndSubregion.Y(2)-results{di}.pMCROfXAndYAndTimeAndSubregion.Y(1);
        timedelta = results{di}.pMCROfXAndYAndTimeAndSubregion.Time(2)-results{di}.pMCROfXAndYAndTimeAndSubregion.Time(1);
        % the following does not integrate to diffuse R
        disp(['Total reflectance captured by pMCROfXAndYAndTimeAndSubregion detector: ' num2str(sum(xdelta*ydelta*timedelta*results{di}.pMCROfXAndYAndTimeAndSubregion.Mean(:)))]);
        % but this does
        disp(['Total reflectance captured by pMCROfXAndYAndTimeAndSubregion detector - ROfXAndY: ' num2str(sum(xdelta*ydelta*results{di}.pMCROfXAndYAndTimeAndSubregion.ROfXAndY(:)))]);
    end
    if isfield(results{di}, 'pMCROfXAndYAndTimeAndSubregionRecessed') && show.pMCROfXAndYAndTimeAndSubregionRecessed
        y0idx = floor(length(results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Y_Midpoints)/2);
        for i=2:results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.NumberOfRegions-1 % exclude air above and below          
          figname = sprintf('log10(%s) region idx=%i',results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Name,i-1); figure; 
          imagesc(results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.X_Midpoints, results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Time_Midpoints,...
            log10(squeeze(results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Mean(i,:,y0idx,:)))); 
          colorbar; title(figname); set(gcf,'Name', figname);ylabel('time [ns]'); xlabel('x [mm]');
        end
        xdelta = results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.X(2)-results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.X(1);        
        ydelta = results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Y(2)-results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Y(1);
        timedelta = results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Time(2)-results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Time(1);
        % the following does not integrate to diffuse R
        disp(['Total reflectance captured by pMCROfXAndYAndTimeAndSubregionRecessed detector: ' num2str(sum(xdelta*ydelta*timedelta*results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.Mean(:)))]);
        % but this does
        disp(['Total reflectance captured by pMCROfXAndYAndTimeAndSubregion detector - ROfXAndY: ' num2str(sum(xdelta*ydelta*results{di}.pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndY(:)))]);

    end
    if isfield(results{di}, 'pMCROfFx') && show.pMCROfFx
        figname = sprintf('%s - Amplitude',results{di}.pMCROfFx.Name);figure;plot(results{di}.pMCROfFx.Fx_Midpoints, abs(results{di}.pMCROfFx.Mean));title(figname);set(gcf,'Name', figname);xlabel('f_x [/mm]');ylabel('R(f_x) [unitless]');
        Fxdelta = results{di}.pMCROfFx.Fx(2)-results{di}.pMCROfFx.Fx(1);
        Fxnorm = 2 * pi * (results{di}.pMCROfFx.Fx_Midpoints * Fxdelta);
        disp(['Total reflectance captured by ROfFx detector: ' num2str(sum(results{di}.pMCROfFx.Mean.*Fxnorm'))]);
    end     
    if isfield(results{di}, 'pMCTOfRho') && show.pMCTOfRho
        figname = sprintf('log10(%s)',results{di}.pMCTOfRho.Name); figure; plot(results{di}.pMCTOfRho.Rho_Midpoints, log10(results{di}.pMCTOfRho.Mean)); title(figname); set(gcf,'Name', figname); xlabel('\rho [mm]'); ylabel('pMC T(\rho) [mm^-^2]');
        disp(['Total reflectance captured by pMCTOfRho detector: ' num2str(sum(results{di}.pMCTOfRho.Mean(:)))]);
    end
  end
end
