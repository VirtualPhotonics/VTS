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
show.ROfRhoAndTimeAndMaxDepth = 1;
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
show.pMCROfRhoAndTimeAndSubregion = 1;
show.pMCROfRhoAndTimeAndSubregionRecessed = 1;
show.pMCROfXAndY =              1;
show.pMCROfXAndYAndTimeAndSubregion = 1;
show.pMCROfXAndYAndTimeAndSubregionRecessed = 1;
show.pMCROfFx =                 1;
show.pMCROfFxAndTime =          1;
show.pMCTOfRho =                1;
show.dMCdROfRhodMua =           1;
show.dMCdROfRhodMus =           1;
show.dMCdROfRhoAndTimedMua =    1;
show.dMCdROfRhoAndTimedMus =    1;
show.dMCdROfFxdMua =            1;
show.dMCdROfFxdMus =            1;
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
      
    result = results{di};

    if isfield(result, 'SurfaceFiber') && isfield(show, 'SurfaceFiber') && show.SurfaceFiber
        d = result.SurfaceFiber;
        fprintf('Total reflectance captured by SurfaceFiber detector: %g\n', d.Mean);
        fprintf('Standard Deviation captured by SurfaceFiber detector: %g\n', d.Stdev);
        fprintf('+/- 3sigma by SurfaceFiber detector: %g - %g\n', ...
            d.Mean - 3 * d.Stdev, d.Mean + 3 * d.Stdev);
    end

    if isfield(result, 'SlantedRecessedFiber') && ...
            isfield(show, 'SlantedRecessedFiber') && show.SlantedRecessedFiber
        d = result.SlantedRecessedFiber;
        fprintf('Total reflectance captured by SlantedRecessedFiber detector: %g\n', d.Mean);
        fprintf('Standard Deviation captured by SlantedRecessedFiber detector: %g\n', d.Stdev);
        fprintf('+/- 3sigma by SlantedRecessedFiber detector: %g - %g\n', ...
            d.Mean - 3 * d.Stdev, d.Mean + 3 * d.Stdev);
    end

    if isfield(result, 'RDiffuse') && isfield(show, 'RDiffuse') && show.RDiffuse
        fprintf('Total reflectance captured by RDiffuse detector: %g\n', ...
            result.RDiffuse.Mean);
    end

    if isfield(result, 'RSpecular') && show.RSpecular
        fprintf('Total reflectance captured by RSpecular detector: %g\n', ...
            result.RSpecular.Mean);
    end

    if isfield(result, 'ROfRho') && show.ROfRho
        d = result.ROfRho;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('R(\rho) [mm^-^2]');
        rhodelta = diff(d.Rho(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * rhodelta;
        fprintf('Total reflectance captured by ROfRho detector: %g\n', ...
            sum(d.Mean .* rhonorm'));
    end
    
    if isfield(results{di}, 'ROfRhoRecessed') && show.ROfRhoRecessed
        d = results{di}.ROfRhoRecessed;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('R(\rho) [mm^-^2]');
        rhodelta = diff(d.Rho(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * rhodelta;
        total = sum(d.Mean .* rhonorm');
        disp(['Total reflectance captured by ROfRhoRecessed detector: ' num2str(total)]);
    end

    if isfield(results{di}, 'ROfAngle') && show.ROfAngle
        d = results{di}.ROfAngle;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Angle_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\angle [rad]');
        ylabel('R(angle) [rad^-^1]');
        angledelta = diff(d.Angle(1:2));
        anglenorm = 2 * pi * sin(d.Angle_Midpoints) * angledelta;
        total = sum(d.Mean .* anglenorm');
        disp(['Total reflectance captured by ROfAngle detector: ' num2str(total)]);
    end

    if isfield(results{di}, 'ROfRhoAndTime') && show.ROfRhoAndTime
        d = results{di}.ROfRhoAndTime;
        rhodelta = diff(d.Rho(1:2));
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * rhodelta;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Time_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('time [ns]');
        total = timedelta * sum(d.Mean .* rhonorm(:).', 'all');
        disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(total)]);
    end

    if isfield(results{di}, 'ROfRhoAndTimeAndMaxDepth') && show.ROfRhoAndTimeAndMaxDepth
        d = results{di}.ROfRhoAndTimeAndMaxDepth;
        rhodelta = diff(d.Rho(1:2));
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * rhodelta;
        figure;
        hold on;
        rhoIndices = 1:10:numel(d.Rho_Midpoints);
        timeIndices = 1:100:numel(d.Time_Midpoints);
        labels = cell(numel(rhoIndices) * numel(timeIndices), 1);
        k = 1;
        for i = rhoIndices
            for j = timeIndices
                plot(d.MaxDepth_Midpoints, d.Mean(:, j, i));
                labels{k} = sprintf('rho=%3.2f mm time=%3.2f ns', ...
                    d.Rho_Midpoints(i), d.Time_Midpoints(j));
                k = k + 1;
            end
        end
        legend(labels);
        title('Max Depth Distribution');
        xlabel('z [mm]');
        ylabel('max depth');
        total = timedelta * sum(bsxfun(@times, d.Mean, reshape(rhonorm, 1, 1, [])), 'all');
        disp(['Total reflectance captured by ROfRhoAndTimeAndMaxDepth detector: ' num2str(total)]);
    end

    if isfield(results{di}, 'ROfRhoAndMaxDepth') && show.ROfRhoAndMaxDepth
        d = results{di}.ROfRhoAndMaxDepth;
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.MaxDepth_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('max depths [mm]');
        xlabel('\rho [mm]');
        figure;
        hold on;
        indices = 1:10:numel(d.Rho_Midpoints);
        for i = indices
            plot(d.MaxDepth_Midpoints, d.Mean(:, i));
        end
        legend(arrayfun(@(i) sprintf('rho=%3.2f mm', d.Rho_Midpoints(i)), ...
            indices, 'UniformOutput', false));
        title('Max Depth Distribution');
        xlabel('z [mm]');
        ylabel('max depth');
        total = sum(d.Mean .* rhonorm(:).', 'all');
        disp(['Total reflectance captured by ROfRhoAndMaxDepth detector: ' num2str(total)]);
    end

    if isfield(results{di}, 'ROfRhoAndMaxDepthRecessed') && show.ROfRhoAndMaxDepthRecessed
        d = results{di}.ROfRhoAndMaxDepthRecessed;
        rhonorm = 2 * pi * d.Rho_Midpoints(:).' * diff(d.Rho(1:2));
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.MaxDepth_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('max depths [mm]');
        indices = 1:10:numel(d.Rho_Midpoints);
        figure;
        plot(d.MaxDepth_Midpoints, d.Mean(:, indices));
        legend(arrayfun(@(i) sprintf('rho=%3.2f mm', d.Rho_Midpoints(i)), ...
            indices, 'UniformOutput', false));
        title('Max Depth Distribution');
        xlabel('z [mm]');
        ylabel('max depth');
        total = sum(d.Mean .* rhonorm, 'all');
        disp(['Total reflectance captured by ROfRhoAndMaxDepthRecessed detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'ROfRhoAndAngle') && show.ROfRhoAndAngle
        d = results{di}.ROfRhoAndAngle;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Angle_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('\angle [rad]');
        xlabel('\rho [mm]');
        rhodelta = diff(d.Rho(1:2));
        angledelta = diff(d.Angle(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints(:).' * rhodelta;
        anglenorm = 2 * pi * sin(d.Angle_Midpoints(:)) * angledelta;
        total = sum(d.Mean .* anglenorm .* rhonorm, 'all');
        disp(['Total reflectance captured by ROfRhoAndAngle detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'ROfRhoAndOmega') && show.ROfRhoAndOmega
        d = results{di}.ROfRhoAndOmega;
        rhonorm = 2 * pi * d.Rho_Midpoints(:).' * diff(d.Rho(1:2));
        figure;
        imagesc(d.Rho_Midpoints, d.Omega_Midpoints, log10(d.Amplitude));
        colorbar;
        title(sprintf('%s - log10(Amplitude)', d.Name));
        set(gcf, 'Name', sprintf('%s - log10(Amplitude)', d.Name));
        xlabel('\rho [mm]');
        ylabel('\omega [GHz]');
        figure;
        imagesc(d.Rho_Midpoints, d.Omega_Midpoints, d.Phase);
        colorbar;
        title(sprintf('%s - Phase', d.Name));
        set(gcf, 'Name', sprintf('%s - Phase', d.Name));
        xlabel('\rho [mm]');
        ylabel('\omega [GHz]');
        total = sum(d.Amplitude(1, :) .* rhonorm, 'all');
        disp(['Total reflectance captured by ROfRhoAndOmega detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'ROfXAndY') && show.ROfXAndY
        d = results{di}.ROfXAndY;
        xynorm = diff(d.X(1:2)) * diff(d.Y(1:2));
        if numel(d.Y_Midpoints) > 1
            figure;
            imagesc(d.Y_Midpoints, d.X_Midpoints, log10(d.Mean));
            colorbar;
            title(sprintf('log10(%s)', d.Name));
            set(gcf, 'Name', sprintf('log10(%s)', d.Name));
            xlabel('X [mm]');
            ylabel('Y [mm]');
        else
            figure;
            plot(d.X_Midpoints, d.Mean(1, :));
            title('log10(R(x,y=0)');
            xlabel('X [mm]');
            ylabel('R(x) [mm^-^2]');
        end
        disp(['Total reflectance captured by ROfXAndY detector: ' ...
            num2str(sum(d.Mean, 'all') * xynorm)]);
        [r, c] = find(d.Mean);
        if ~isempty(r)
            fprintf('ROfXAndY: x non-zero span [%d %d]\n', min(r), max(r));
            fprintf('ROfXAndY: y non-zero span [%d %d]\n', min(c), max(c));
        end
    end

    if isfield(results{di}, 'ROfXAndYRecessed') && show.ROfXAndYRecessed
        d = results{di}.ROfXAndYRecessed;
        xynorm = diff(d.X(1:2)) * diff(d.Y(1:2));
        figure;
        imagesc(d.Y_Midpoints, d.X_Midpoints, log10(d.Mean));
        colorbar;
        title(sprintf('log10(%s)', d.Name));
        set(gcf, 'Name', sprintf('log10(%s)', d.Name));
        xlabel('X [mm]');
        ylabel('Y [mm]');
        fprintf('Total reflectance captured by ROfXAndYRecessed detector: %g\n', ...
            sum(d.Mean, 'all') * xynorm);
        [r, c] = find(d.Mean);
        if ~isempty(r)
            fprintf('ROfXAndYRecessed: x non-zero span [%d %d]\n', min(r), max(r));
            fprintf('ROfXAndYRecessed: y non-zero span [%d %d]\n', min(c), max(c));
        end
    end

    if isfield(results{di}, 'ROfXAndYAndTime') && show.ROfXAndYAndTime
        d = results{di}.ROfXAndYAndTime;
        for i = 1:10:numel(d.Time)-1
            figname = sprintf('log10(%s) time=%5.3f ns', d.Name, d.Time_Midpoints(i));
            figure;
            imagesc(d.X_Midpoints, d.Y_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('y [mm]');
        end
        binvol = diff(d.X(1:2)) * diff(d.Y(1:2)) * diff(d.Time(1:2));
        fprintf('Total reflectance captured by ROfXAndYAndTime detector: %g\n', ...
            sum(d.Mean, 'all') * binvol);
    end

    if isfield(results{di}, 'ROfXAndYAndTimeRecessed') && ...
            show.ROfXAndYAndTimeRecessed
        d = results{di}.ROfXAndYAndTimeRecessed;
        for i = 1:10:numel(d.Time)-1
            figname = sprintf('log10(%s) time=%5.3f ns', d.Name, d.Time_Midpoints(i));
            figure;
            imagesc(d.X_Midpoints, d.Y_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('y [mm]');
        end
        binvol = diff(d.X(1:2)) * diff(d.Y(1:2)) * diff(d.Time(1:2));
        fprintf('Total reflectance captured by ROfXAndYAndTimeRecessed detector: %g\n', ...
            sum(d.Mean, 'all') * binvol);
    end

    if isfield(results{di}, 'ROfXAndYAndTimeAndSubregion') && ...
            show.ROfXAndYAndTimeAndSubregion
        d = results{di}.ROfXAndYAndTimeAndSubregion;
        y0idx = floor(numel(d.Y_Midpoints) / 2);
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i - 1);
            figure;
            imagesc(d.X_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, y0idx, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('time [ns]');
        end
        binvol = diff(d.X(1:2)) * diff(d.Y(1:2)) * diff(d.Time(1:2));
        % the following does not integrate to diffuse R
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, binvol * sum(d.Mean, 'all'));
        fprintf('Total reflectance captured by %s detector - ROfXAndY: %g\n', ...
            d.Name, diff(d.X(1:2)) * diff(d.Y(1:2)) * sum(d.ROfXAndY, 'all'));
    end

    if isfield(results{di}, 'ROfXAndYAndTimeAndSubregionRecessed') && ...
            show.ROfXAndYAndTimeAndSubregionRecessed
        d = results{di}.ROfXAndYAndTimeAndSubregionRecessed;
        y0idx = floor(numel(d.Y_Midpoints) / 2);
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i - 1);
            figure;
            imagesc(d.X_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, y0idx, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('time [ns]');
        end
        binvol = diff(d.X(1:2)) * diff(d.Y(1:2)) * diff(d.Time(1:2));
        % the following does not integrate to diffuse R
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, binvol * sum(d.Mean, 'all'));
        fprintf('Total reflectance captured by %s detector - ROfXAndY: %g\n', ...
            d.Name, diff(d.X(1:2)) * diff(d.Y(1:2)) * sum(d.ROfXAndY, 'all'));
    end

    if isfield(results{di}, 'ROfXAndYAndThetaAndPhi') && ...
            show.ROfXAndYAndThetaAndPhi
        d = results{di}.ROfXAndYAndThetaAndPhi;
        yidx = floor(numel(d.Y_Midpoints) / 2);
        xidx = floor(numel(d.X_Midpoints) / 2);
        figname = sprintf('log10(%s) x=%5.3f y=%5.3f', d.Name, ...
            d.X(xidx), d.Y(yidx));
        figure;
        imagesc(d.Phi_Midpoints, d.Theta_Midpoints, ...
            log10(squeeze(d.Mean(:, :, yidx, xidx))));
        colormap(jet);
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('phi [radians]');
        ylabel('theta [radians]');
        xyphinorm = diff(d.X(1:2)) * diff(d.Y(1:2)) * diff(d.Phi(1:2));
        partialsum = xyphinorm * sum(d.Mean, [1, 3, 4]);
        thetanorm = sin(d.Theta_Midpoints) * diff(d.Theta(1:2));
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, sum(partialsum .* thetanorm));
    end
    
    if isfield(results{di}, 'ROfXAndYAndMaxDepth') && ...
            show.ROfXAndYAndMaxDepth
        d = results{di}.ROfXAndYAndMaxDepth;
        numdepths = numel(d.MaxDepth) - 1;
        for i = 1:10:numdepths
            figname = sprintf('log10(%s) depth=%5.3f mm', ...
                d.Name, d.MaxDepth_Midpoints(i));
            figure;
            imagesc(d.X_Midpoints, d.Y_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('y [mm]');
        end
        binvol = diff(d.X(1:2)) * diff(d.Y(1:2));
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, binvol * sum(d.Mean, 'all'));
    end

    if isfield(results{di}, 'ROfXAndYAndMaxDepthRecessed') && ...
            show.ROfXAndYAndMaxDepthRecessed
        d = results{di}.ROfXAndYAndMaxDepthRecessed;
        numdepths = numel(d.MaxDepth) - 1;
        for i = 1:10:numdepths
            figname = sprintf('log10(%s) depth=%5.3f mm', ...
                d.Name, d.MaxDepth_Midpoints(i));
            figure;
            imagesc(d.X_Midpoints, d.Y_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('y [mm]');
        end
        binvol = diff(d.X(1:2)) * diff(d.Y(1:2));
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, binvol * sum(d.Mean, 'all'));
    end

    if isfield(results{di}, 'ROfFx') && show.ROfFx
        d = results{di}.ROfFx;
        figname = d.Name;
        figure;
        plot(d.Fx_Midpoints, abs(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('f_x [/mm]');
        ylabel('R(f_x) [unitless]');
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, d.Amplitude(1));
    end

    if isfield(results{di}, 'ROfFxAndTime') && show.ROfFxAndTime
        d = results{di}.ROfFxAndTime;
        figname = sprintf('%s - log10(Amplitude)', d.Name);
        figure;
        imagesc(d.Time_Midpoints, d.Fx_Midpoints, log10(d.Amplitude'));
        title(figname);
        set(gcf, 'Name', figname);
        colorbar;
        xlabel('time [ns]');
        ylabel('f_x [/mm]');
        timedelta = diff(d.Time(1:2));
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, timedelta * sum(d.Amplitude(:, 1)));
    end

    if isfield(results{di}, 'ROfFxAndAngle') && show.ROfFxAndAngle
        d = results{di}.ROfFxAndAngle;
        figname = sprintf('%s - log10(Amplitude)', d.Name);
        figure;
        imagesc(d.Angle_Midpoints, d.Fx_Midpoints, log10(d.Amplitude'));
        title(figname);
        set(gcf, 'Name', figname);
        colorbar;
        xlabel('angle [rad]');
        ylabel('f_x [/mm]');

        angledelta = diff(d.Angle(1:2));
        anglenorm = 2 * pi * sin(d.Angle_Midpoints) * angledelta;
        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, sum(anglenorm * d.Amplitude(:, 1)));
    end

    if isfield(results{di}, 'ROfFxAndMaxDepth') && show.ROfFxAndMaxDepth
        d = results{di}.ROfFxAndMaxDepth;
        figname = sprintf('%s - log10(Amplitude)', d.Name);
        figure;
        imagesc(d.MaxDepth_Midpoints, d.Fx_Midpoints, log10(d.Amplitude'));
        title(figname);
        set(gcf, 'Name', figname);
        colorbar;
        xlabel('depth [mm]');
        ylabel('f_x [/mm]');

        fprintf('Total reflectance captured by %s detector: %g\n', ...
            d.Name, sum(d.Amplitude(:, 1)));
    end

    if isfield(results{di}, 'TDiffuse') && show.TDiffuse
        d = results{di}.TDiffuse;
        fprintf('Total transmittance captured by TDiffuse detector: %g\n', d.Mean);
    end

    if isfield(results{di}, 'TOfRho') && show.TOfRho
        d = results{di}.TOfRho;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('T(\rho) [mm^{-2}]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        fprintf('Total transmittance captured by TOfRho detector: %g\n', ...
            sum(d.Mean(:) .* rhonorm(:)));
    end

    if isfield(results{di}, 'TOfAngle') && show.TOfAngle
        d = results{di}.TOfAngle;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Angle_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\angle [rad]');
        ylabel('T(angle) [rad^{-1}]');
        anglenorm = 2 * pi * sin(d.Angle_Midpoints) * diff(d.Angle(1:2));
        fprintf('Total transmittance captured by TOfAngle detector: %g\n', ...
            sum(d.Mean(:) .* anglenorm(:)));
    end

    if isfield(results{di}, 'TOfRhoAndAngle') && show.TOfRhoAndAngle
        d = results{di}.TOfRhoAndAngle;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Angle_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('\angle [rad]');
        xlabel('\rho [mm]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        anglenorm = 2 * pi * sin(d.Angle_Midpoints) * diff(d.Angle(1:2));
        fprintf('Total transmittance captured by TOfRhoAndAngle detector: %g\n', ...
            sum(d.Mean .* (anglenorm(:) * rhonorm(:).'), 'all'));
    end

    if isfield(results{di}, 'TOfXAndY') && show.TOfXAndY
        d = results{di}.TOfXAndY;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Y_Midpoints, d.X_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('Y [mm]');
        xlabel('X [mm]');
        xynorm = diff(d.X(1:2)) * diff(d.Y(1:2));
        fprintf('Total transmittance captured by TOfXAndY detector: %g\n', ...
            xynorm * sum(d.Mean, 'all'));
        % determine range of x, y midpoints thathave non-zero data
        [rows, cols] = find(d.Mean);
        if ~isempty(rows)
            fprintf('TOfXAndY: x non-zero span [%d %d]\n', min(rows), max(rows));
            fprintf('TOfXAndY: y non-zero span [%d %d]\n', min(cols), max(cols));
        end
    end

    if isfield(results{di}, 'TOfXAndYAndTimeAndSubregion') && show.TOfXAndYAndTimeAndSubregion
        d = results{di}.TOfXAndYAndTimeAndSubregion;
        y0idx = floor(numel(d.Y_Midpoints) / 2);
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i - 1);
            figure;
            imagesc(d.X_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, y0idx, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('time [ns]');
        end
        voxelVolume = prod([diff(d.X(1:2)), diff(d.Y(1:2)), diff(d.Time(1:2))]);
        fprintf('Total transmittance captured by %s detector: %g\n', ...
            d.Name, voxelVolume * sum(d.Mean, 'all'));
        fprintf('Total transmittance captured by %s detector - TOfXAndY: %g\n', ...
            d.Name, prod([diff(d.X(1:2)), diff(d.Y(1:2))]) * sum(d.TOfXAndY, 'all'));
    end

    if isfield(results{di}, 'TOfFx') && show.TOfFx
        d = results{di}.TOfFx;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Fx_Midpoints, abs(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('f_x [/mm]');
        ylabel('T(f_x) [unitless]');
        fprintf('Total transmittance captured by TOfFx detector: %g\n', d.Amplitude(1));
    end

    if isfield(results{di}, 'ATotal') && show.ATotal
        fprintf('Total absorption captured by ATotal detector: %g\n', ...
            results{di}.ATotal.Mean);
    end

    if isfield(results{di}, 'AOfRhoAndZ') && show.AOfRhoAndZ
        d = results{di}.AOfRhoAndZ;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Z_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('z [mm]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        fprintf('Absorbed energy captured by %s detector: %g\n', ...
            d.Name, diff(d.Z(1:2)) * sum(d.Mean .* rhonorm, 'all'));
    end

    if isfield(results{di}, 'AOfXAndYAndZ') && show.AOfXAndYAndZ
        d = results{di}.AOfXAndYAndZ;
        center = floor((numel(d.Y) - 1) / 2) + 1;
        figname = sprintf('log10(%s) y=%5.3f mm', d.Name, d.Y_Midpoints(center));
        figure;
        imagesc(d.X_Midpoints, d.Z_Midpoints, log10(squeeze(d.Mean(:, center, :))));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('x [mm]');
        ylabel('z [mm]');
        voxelVolume = prod([diff(d.X(1:2)), diff(d.Y(1:2)), diff(d.Z(1:2))]);
        fprintf('Absorbed energy captured by %s detector: %g\n', ...
            d.Name, voxelVolume * sum(d.Mean, 'all'));
    end

    if isfield(results{di}, 'ATotalBoundingVolume') && show.ATotalBoundingVolume
        fprintf('Total absorption captured by ATotalBoundingVolume detector: %g\n', ...
            results{di}.ATotalBoundingVolume.Mean);
    end

    if isfield(results{di}, 'FluenceOfRhoAndZ') && show.FluenceOfRhoAndZ
        d = results{di}.FluenceOfRhoAndZ;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Z_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('z [mm]');
        colormap(jet);
        rhoNorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        fprintf('Fluence captured by %s detector: %g\n', ...
            d.Name, diff(d.Z(1:2)) * sum(d.Mean .* rhoNorm, 'all'));
    end

    if isfield(results{di}, 'FluenceOfRhoAndZAndTime') && show.FluenceOfRhoAndZAndTime
        d = results{di}.FluenceOfRhoAndZAndTime;
        numtimes = numel(d.Time) - 1;
        for i = 1:10:numtimes
            figname = sprintf('log10(%s) time=%5.3f ns', ...
                d.Name, d.Time_Midpoints(i));
            figure;
            imagesc(d.Rho_Midpoints, d.Z_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('z [mm]');
            xlabel('\rho [mm]');
        end
        drho = diff(d.Rho(1:2));
        dt = diff(d.Time(1:2));
        dz = diff(d.Z(1:2));
        rhoWeights = reshape(2 * pi * d.Rho_Midpoints * drho, 1, 1, []);
        totalFluence = dt * dz * sum(d.Mean .* rhoWeights, 'all');
        fprintf('Fluence captured by %s detector: %g\n', d.Name, totalFluence);
    end

    if isfield(results{di}, 'FluenceOfRhoAndZAndOmega') && show.FluenceOfRhoAndZAndOmega
        d = results{di}.FluenceOfRhoAndZAndOmega;
        numzs = numel(d.Z) - 1;
        for i = 1:10:numel(d.Omega_Midpoints)
            figname = sprintf('log10(%s:amplitude) omega=%5.3f GHz', ...
                d.Name, d.Omega_Midpoints(i));
            figure;
            imagesc(d.Rho_Midpoints, d.Z_Midpoints, log10(squeeze(d.Amplitude(i, :, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('\rho [mm]');
            ylabel('z [mm]');
        end
        rhoNorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        rhoMatrix = repmat(rhonorm',[1,numzs]);
        totalFluence = diff(d.Z(1:2)) * sum( ...
            squeeze(d.Amplitude(1, :, :)) .* permute(rhoMatrix,[2,1]),'all');
        fprintf('Fluence captured by %s detector at omega=%5.3f GHz: %5.3f\n', ...
            d.Name, d.Omega_Midpoints(1), totalFluence);
    end

    if isfield(results{di}, 'FluenceOfXAndYAndZ') && show.FluenceOfXAndYAndZ
        d = results{di}.FluenceOfXAndYAndZ;
        center = floor((numel(d.Y) - 1) / 2) + 1;
        figname = sprintf('log10(%s) y=%5.3f mm', d.Name, d.Y_Midpoints(center));
        figure;
        imagesc(d.X_Midpoints, d.Z_Midpoints, log10(squeeze(d.Mean(:, center, :))));
        colormap(jet);
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('x [mm]');
        ylabel('z [mm]');
        voxelVolume = prod([diff(d.X(1:2)), diff(d.Y(1:2)), diff(d.Z(1:2))]);
        fprintf('Fluence captured by %s detector: %g\n', d.Name, ...
            voxelVolume * sum(d.Mean, 'all'));
    end

    if isfield(results{di}, 'FluenceOfXAndYAndZAndTime') && show.FluenceOfXAndYAndZAndTime
        d = results{di}.FluenceOfXAndYAndZAndTime;
        center = floor((numel(d.Y) - 1) / 2) + 1;
        for i = 1:10:numel(d.Time_Midpoints)
            figname = sprintf('log10(%s) y=0 time=%5.3f ns', ...
                d.Name, d.Time_Midpoints(i));
            figure;
            imagesc(d.X_Midpoints, d.Z_Midpoints, ...
                log10(squeeze(d.Mean(i, :, center, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('z [mm]');
        end
        voxelVolume = prod([diff(d.X(1:2)), diff(d.Y(1:2)), diff(d.Z(1:2))]);
        fprintf('Fluence captured by %s detector at Time=%5.3f ns: %5.3f\n', ...
            d.Name, d.Time_Midpoints(1), voxelVolume * sum(d.Mean(1, :, :, :), 'all'));
    end

    if isfield(results{di}, 'FluenceOfXAndYAndZAndOmega') && show.FluenceOfXAndYAndZAndOmega
        d = results{di}.FluenceOfXAndYAndZAndOmega;
        center = floor((numel(d.Y) - 1) / 2) + 1;
        for i = 1:10:numel(d.Omega_Midpoints)
            figname = sprintf('log10(%s:amplitude) y=0 omega=%5.3f GHz', ...
                d.Name, d.Omega_Midpoints(i));
            figure;
            imagesc(d.X_Midpoints, d.Z_Midpoints, ...
                log10(squeeze(d.Amplitude(i, :, center, :))));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('z [mm]');
        end
        voxelVolume = prod([diff(d.X(1:2)), diff(d.Y(1:2)), diff(d.Z(1:2))]);
        fprintf('Fluence captured by %s detector at omega=%5.3f GHz: %5.3f\n', ...
            d.Name, d.Omega_Midpoints(1), voxelVolume * sum(d.Amplitude(1, :, :, :), 'all'));
    end

    if isfield(results{di}, 'FluenceOfXAndYAndZAndStartingXAndY') && ...
            show.FluenceOfXAndYAndZAndStartingXAndY
        d = results{di}.FluenceOfXAndYAndZAndStartingXAndY;
        center = floor((numel(d.Y) - 1) / 2) + 1;
        figname = sprintf('log10(Fluence(X,Y,Z)) StartingX=%5.3f StartingY=%5.3f', ...
            d.StartingX_Midpoints(1), d.StartingY_Midpoints(1));
        figure;
        imagesc(d.X_Midpoints, d.Z_Midpoints, ...
            log10(squeeze(d.Mean(:, center, :, 1, 1))));
        colormap(jet);
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('z [mm]');
        xlabel('x [mm]');
        fprintf('# photons starting in X=3, Y=1: %d\n', d.StartingXYCount(1, 1));
        voxelVolume = prod([diff(d.X(1:2)), diff(d.Y(1:2)), diff(d.Z(1:2))]);
        fprintf('Fluence captured by %s: %5.3f\n', d.Name, ...
            voxelVolume * sum(d.Mean, 'all'));
    end

    if isfield(results{di}, 'FluenceOfFxAndZ') && show.FluenceOfFxAndZ
        d = results{di}.FluenceOfFxAndZ;
        figname = sprintf('log10(%s:amplitude)', d.Name);
        figure;
        imagesc(d.Fx_Midpoints, d.Z_Midpoints, log10(d.Amplitude));
        colormap(jet);
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('z [mm]');
        xlabel('fx [/mm]');
        fprintf('Fluence captured by %s detector at fx=0: %5.3f\n', ...
            d.Name, diff(d.Z(1:2)) * sum(d.Amplitude(:, 1)));
    end

    if isfield(results{di}, 'RadianceOfRhoAndZAndAngle') && ...
            show.RadianceOfRhoAndZAndAngle
        d = results{di}.RadianceOfRhoAndZAndAngle;
        numAngles = numel(d.Angle) - 1;
        % create colorbar based in max, min values
        minRadiance = max(min(d.Mean(:)), 1e-5); % make sure don't take log of 0
        maxRadiance = max(d.Mean(:));
        for i = 1:numAngles
            figname = sprintf('log10(%s) %5.3f<angle<%5.3f', ...
                d.Name, (i - 1) * pi / numAngles, i * pi / numAngles);
            figure;
            imagesc(d.Rho_Midpoints, d.Z_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colormap(jet);
            colorbar;
            caxis([log10(minRadiance), log10(maxRadiance)]);
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('z [mm]');
            xlabel('\rho [mm]');
        end
        if numAngles == 2
            figname = 'log10(lower./upper)';
            figure;
            imagesc(d.Rho_Midpoints, d.Z_Midpoints, ...
                log10(squeeze(d.Mean(1, :, :) ./ d.Mean(2, :, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('z [mm]');
            xlabel('\rho [mm]');
        end
        rhoNorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        angleNorm = 2 * pi * sin(d.Angle_Midpoints) * diff(d.Angle(1:2));
        weightedMean = d.Mean .* reshape(angleNorm, [], 1, 1) .* ...
            reshape(rhoNorm, 1, 1, []);
        fprintf('Radiance captured by %s detector: %g\n', d.Name, ...
            diff(d.Z(1:2)) * sum(weightedMean, 'all'));
    end

    if isfield(results{di}, 'RadianceOfFxAndZAndAngle') && ...
            show.RadianceOfFxAndZAndAngle
        d = results{di}.RadianceOfFxAndZAndAngle;
        numAngles = numel(d.Angle) - 1;
        numZ = numel(d.Z) - 1;
        angleDelta = diff(d.Angle(1:2));
        zDelta = diff(d.Z(1:2));
        angleNorm = 2 * pi * sin(d.Angle_Midpoints) * angleDelta;
        plotIndices = 1:10:min(51, numel(d.Fx_Midpoints));
        for i = 1:numAngles
            angleLimits = [(i - 1), i] * pi / numAngles;
            figname = sprintf('log10(%s) amplitude %5.3f<angle<%5.3f', ...
                d.Name, angleLimits);
            figure;
            imagesc(d.Fx_Midpoints, d.Z_Midpoints, ...
                log10(max(abs(squeeze(d.Amplitude(i, :, :))), eps)));
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('z [mm]');
            xlabel('fx [/mm]');
            figure;
            hold on;
            for j = plotIndices
                plot(d.Z_Midpoints, ...
                    log10(max(abs(squeeze(d.Amplitude(i, :, j))), eps)), ...
                    'DisplayName', sprintf('f_x = %g', d.Fx_Midpoints(j)));
            end
            hold off;
            title(sprintf('%5.3f<angle<%5.3f', angleLimits));
            ylabel('log Radiance amplitude');
            xlabel('z [mm]');
            legend('show');
            set(gcf, 'Name', sprintf('%s line scans', figname));
        end
        captured = zDelta * sum(d.Amplitude(:, :, 1) .* angleNorm(:), 'all');
        fprintf('Radiance captured by %s detector for fx=0: %g\n', ...
            d.Name, captured);
    end

    if isfield(results{di}, 'RadianceOfXAndYAndZAndThetaAndPhi') && ...
            show.RadianceOfXAndYAndZAndThetaAndPhi
        d = results{di}.RadianceOfXAndYAndZAndThetaAndPhi;
        numThetas = numel(d.Theta) - 1;
        for i = 1:numThetas
            thetaLimits = [(i - 1), i] * pi / numThetas;
            figname = sprintf('log10(%s) %5.3f<Theta<%5.3f', ...
                d.Name, thetaLimits);
            figure;
            imagesc(d.X_Midpoints, d.Z_Midpoints, ...
                log10(squeeze(d.Mean(1, i, :, 1, :))), [-20, -5]);
            colormap(jet);
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('z [mm]');
            xlabel('x [mm]');
        end
        spatialDelta = prod([diff(d.X(1:2)), diff(d.Y(1:2)), ...
            diff(d.Z(1:2)), diff(d.Phi(1:2))]);
        thetaWeights = reshape(sin(d.Theta_Midpoints) * diff(d.Theta(1:2)), ...
            1, [], 1, 1, 1);
        captured = spatialDelta * sum(d.Mean .* thetaWeights, 'all');
        fprintf('Radiance captured by %s detector: %g\n', d.Name, captured);
    end

    if isfield(results{di}, 'ReflectedMTOfRhoAndSubregionHist') && ...
            show.ReflectedMTOfRhoAndSubregionHist
        d = results{di}.ReflectedMTOfRhoAndSubregionHist;
        numRhos = numel(d.Rho) - 1;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.MTBins_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        xlabel('\rho [mm]');
        ylabel('MT');
        set(gcf, 'Name', figname);
        colormap(jet);
        % Dimensions: [fractional MT bins, subregions, MT bins, rho bins]
        for j = 2:min(3, numel(d.SubregionIndices))
            for i = 1:20:numRhos
                figname = sprintf( ...
                    'Reflected Fractional MT in Region %2d, Rho = %5.3f mm', ...
                    j - 1, d.Rho_Midpoints(i));
                layerfrac = squeeze(d.FractionalMT(:, j, :, i));
                figure;
                bar(d.MTBins_Midpoints, layerfrac', 'stacked');
                title(figname);
                xlabel('MT');
                ylabel('photon weight');
                numFracs = size(layerfrac, 1);
                labels = cell(1, numFracs);
                labels{1} = '=0';
                labels{numFracs} = '=1';
                if numFracs > 2
                    edges = linspace(0, 1, numFracs - 1);
                    for k = 2:numFracs - 1
                        labels{k} = sprintf('[%3.2f-%3.2f]', ...
                            edges(k - 1), edges(k));
                    end
                end
                legend(labels);
            end
        end
    end

    if isfield(results{di}, 'ReflectedMTOfXAndYAndSubregionHist') && ...
            show.ReflectedMTOfXAndYAndSubregionHist
        d = results{di}.ReflectedMTOfXAndYAndSubregionHist;
        numXs = numel(d.X) - 1;
        numSubregions = numel(d.SubregionIndices);
        figure;
        imagesc(d.X_Midpoints, d.MTBins_Midpoints, ...
            log10(squeeze(sum(d.Mean, 2))));
        colorbar;
        title(sprintf('log10(%s) summed over y', d.Name));
        xlabel('x [mm]');
        ylabel('MT');
        set(gcf, 'Name', sprintf('log10(%s) summed over y', d.Name));
        colormap(jet);
        xIndices = floor(numXs / 2):min(floor(numXs / 2) + 1, numXs);
        for j = 2:min(3, numSubregions)
            for i = xIndices
                layerfrac = squeeze(d.FractionalMT(:, j, :, 1, i));
                figname = sprintf( ...
                    'Reflected Fractional MT in Region %2d, X = %5.3f mm', ...
                    j - 1, d.X_Midpoints(i));
                figure;
                bar(d.MTBins_Midpoints, layerfrac', 'stacked');
                title(figname);
                xlabel('MT');
                ylabel('photon weight');
                set(gcf, 'Name', figname);
                numFracs = size(layerfrac, 1);
                labels = cell(1, numFracs);
                labels{1} = '=0';
                if numFracs > 1
                    labels{end} = '=1';
                    edges = linspace(0, 1, numFracs - 1);
                    for k = 2:numFracs - 1
                        labels{k} = sprintf('[%3.2f-%3.2f]', ...
                            edges(k - 1), edges(k));
                    end
                end
                legend(labels);
            end
        end
    end

    if isfield(results{di}, 'TransmittedMTOfRhoAndSubregionHist') && ...
            show.TransmittedMTOfRhoAndSubregionHist
        d = results{di}.TransmittedMTOfRhoAndSubregionHist;
        numRhos = numel(d.Rho) - 1;
        numSubregions = numel(d.SubregionIndices);
        figure;
        imagesc(d.Rho_Midpoints, d.MTBins_Midpoints, log10(d.Mean));
        colorbar;
        title(sprintf('log10(%s)', d.Name));
        xlabel('\rho [mm]');
        ylabel('MT');
        set(gcf, 'Name', sprintf('log10(%s)', d.Name));
        colormap(jet);
        for j = 2:min(3, numSubregions)
            for i = 1:20:numRhos
                layerfrac = squeeze(d.FractionalMT(:, j, :, i));
                figname = sprintf( ...
                    'Transmitted Fractional MT in Region %2d, Rho = %5.3f mm', ...
                    j - 1, d.Rho_Midpoints(i));
                figure;
                bar(d.MTBins_Midpoints, layerfrac', 'stacked');
                title(figname);
                xlabel('MT');
                ylabel('photon weight');
                set(gcf, 'Name', figname);
                numFracs = size(layerfrac, 1);
                labels = cell(1, numFracs);
                labels{1} = '=0';
                if numFracs > 1
                    labels{end} = '=1';
                    edges = linspace(0, 1, numFracs - 1);
                    for k = 2:numFracs - 1
                        labels{k} = sprintf('[%3.2f-%3.2f]', ...
                            edges(k - 1), edges(k));
                    end
                end
                legend(labels);
            end
        end
    end

    if isfield(results{di}, 'TransmittedMTOfXAndYAndSubregionHist') && ...
            show.TransmittedMTOfXAndYAndSubregionHist
        d = results{di}.TransmittedMTOfXAndYAndSubregionHist;
        numXs = numel(d.X) - 1;
        numSubregions = numel(d.SubregionIndices);
        figure;
        imagesc(d.X_Midpoints, d.MTBins_Midpoints, ...
            log10(squeeze(sum(d.Mean, 2))));
        colorbar;
        title(sprintf('log10(%s) summed over y', d.Name));
        xlabel('x [mm]');
        ylabel('MT');
        set(gcf, 'Name', sprintf('log10(%s) summed over y', d.Name));
        colormap(jet);
        xIndices = floor(numXs / 2):min(floor(numXs / 2) + 1, numXs);
        for j = 2:min(3, numSubregions)
            for i = xIndices
                layerfrac = squeeze(d.FractionalMT(:, j, :, 1, i));
                figname = sprintf( ...
                    'Transmitted Fractional MT in Region %2d, X = %5.3f mm', ...
                    j - 1, d.X_Midpoints(i));
                figure;
                bar(d.MTBins_Midpoints, layerfrac', 'stacked');
                title(figname);
                xlabel('MT');
                ylabel('photon weight');
                set(gcf, 'Name', figname);
                numFracs = size(layerfrac, 1);
                labels = cell(1, numFracs);
                labels{1} = '=0';
                if numFracs > 1
                    labels{end} = '=1';
                    edges = linspace(0, 1, numFracs - 1);
                    for k = 2:numFracs - 1
                        labels{k} = sprintf('[%3.2f-%3.2f]', ...
                            edges(k - 1), edges(k));
                    end
                end
                legend(labels);
            end
        end
    end

    if isfield(results{di}, 'ReflectedDynamicMTOfRhoAndSubregionHist') && ...
            show.ReflectedDynamicMTOfRhoAndSubregionHist
        d = results{di}.ReflectedDynamicMTOfRhoAndSubregionHist;
        numRhos = numel(d.Rho) - 1;
        figure;
        imagesc(d.Rho_Midpoints, d.MTBins_Midpoints, log10(d.Mean));
        colorbar;
        title(sprintf('log10(%s)', d.Name));
        xlabel('\rho [mm]');
        ylabel('Dynamic MT');
        set(gcf, 'Name', sprintf('log10(%s)', d.Name));
        colormap(jet);
        for i = 1:20:numRhos
            figure;
            figname = sprintf( ...
                'Reflected Fractional Dynamic MT, Rho = %5.3f mm', ...
                d.Rho_Midpoints(i));
            layerfrac = squeeze(d.FractionalMT(:, :, i));
            bar(d.MTBins_Midpoints, layerfrac', 'stacked');
            title(figname);
            xlabel('Dynamic MT');
            ylabel('photon weight');
            set(gcf, 'Name', figname);
            numFracs = size(layerfrac, 1);
            labels = repmat({''}, 1, numFracs);
            labels{1} = '=0';
            if numFracs > 1
                labels{end} = '=1';
                edges = linspace(0, 1, numFracs - 1);
                for k = 2:numFracs - 1
                    labels{k} = sprintf('[%3.2f-%3.2f]', ...
                        edges(k - 1), edges(k));
                end
            end
            legend(labels);
            figure;
            figname = sprintf( ...
                'Reflected Total MT of Z, Rho = %5.3f mm', ...
                d.Rho_Midpoints(i));
            errorbar(d.Z_Midpoints, d.TotalMTOfZ(:, i), ...
                d.TotalMTOfZStdev(:, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Total MT');
            figure;
            figname = sprintf( ...
                'Reflected Dynamic MT of Z, Rho = %5.3f mm', ...
                d.Rho_Midpoints(i));
            errorbar(d.Z_Midpoints, d.DynamicMTOfZ(:, i), ...
                d.DynamicMTOfZStdev(:, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Dynamic MT');
        end
        figure;
        plot(d.SubregionIndices, d.SubregionCollisions(1, :), ...
             d.SubregionIndices, d.SubregionCollisions(2, :));
        title('Reflected Dynamic MT Of Rho: Subregion Collisions');
        xlabel('tissue region index');
        ylabel('Collisions');
        legend('static', 'dynamic');
    end

    if isfield(results{di}, 'ReflectedDynamicMTOfXAndYAndSubregionHist') && ...
            show.ReflectedDynamicMTOfXAndYAndSubregionHist
        d = results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist;
        numxs = numel(d.X) - 1;
        numys = numel(d.Y) - 1;
        figure;
        imagesc(d.X_Midpoints, d.MTBins_Midpoints, ...
            log10(squeeze(sum(d.Mean, 2))));
        colorbar;
        title(sprintf('log10(%s) summed over y', d.Name));
        xlabel('x [mm]');
        ylabel('Dynamic MT');
        set(gcf, 'Name', sprintf('log10(%s) summed over y', d.Name));
        xcenter = max(1, floor(numxs / 2));
        ycenter = max(1, floor(numys / 2));
        for i = xcenter:min(xcenter + 1, numxs)
            figname = sprintf( ...
                'Reflected Fractional Dynamic MT, X = %5.3f mm, Y = %5.3f mm', ...
                d.X_Midpoints(i), d.Y_Midpoints(ycenter));
            figure;
            layerfrac = squeeze(d.FractionalMT(:, :, 1, i));
            bar(d.MTBins_Midpoints, layerfrac', 'stacked');
            title(figname);
            xlabel('Dynamic MT');
            ylabel('photon weight');
            set(gcf, 'Name', figname);
            numFracs = size(layerfrac, 1);
            labels = repmat({''}, 1, numFracs);
            labels{1} = '=0';
            if numFracs > 1
                labels{end} = '=1';
                edges = linspace(0, 1, numFracs - 1);
                for k = 2:numFracs - 1
                    labels{k} = sprintf('[%3.2f-%3.2f]', ...
                        edges(k - 1), edges(k));
                end
            end
            legend(labels);
            figname = sprintf( ...
                'Reflected Total MT of Z, X = %5.3f mm, Y = %5.3f mm', ...
                d.X_Midpoints(i), d.Y_Midpoints(ycenter));
            figure;
            plot(d.Z_Midpoints, d.TotalMTOfZ(:, 1, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Total MT');
            set(gcf, 'Name', figname);
            figname = sprintf( ...
                'Reflected Dynamic MT of Z, X = %5.3f mm, Y = %5.3f mm', ...
                d.X_Midpoints(i), d.Y_Midpoints(ycenter));
            figure;
            plot(d.Z_Midpoints, d.DynamicMTOfZ(:, 1, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Dynamic MT');
            set(gcf, 'Name', figname);
        end
        figure;
        plot(d.SubregionIndices, d.SubregionCollisions(1, :), ...
             d.SubregionIndices, d.SubregionCollisions(2, :));
        title('Reflected Dynamic MT Of X and Y: Subregion Collisions');
        xlabel('tissue region index');
        ylabel('Collisions');
        legend('static', 'dynamic');
    end

    if isfield(results{di}, 'ReflectedDynamicMTOfFxAndSubregionHist') && ...
            show.ReflectedDynamicMTOfFxAndSubregionHist
        d = results{di}.ReflectedDynamicMTOfFxAndSubregionHist;
        numFxs = numel(d.Fx_Midpoints);
        figure;
        imagesc(d.Fx_Midpoints, d.MTBins_Midpoints, log10(abs(d.Mean)));
        colorbar;
        title(sprintf('log10(%s)', d.Name));
        xlabel('Fx [/mm]');
        ylabel('Dynamic MT');
        set(gcf, 'Name', sprintf('log10(%s)', d.Name));
        colormap(jet);
        for i = 1:10:numFxs
            figname = sprintf('Reflected Fractional Dynamic MT, Fx = %5.3f mm', ...
                d.Fx_Midpoints(i));
            layerfrac = squeeze(abs(d.FractionalMT(:, :, i)));
            figure;
            bar(d.MTBins_Midpoints, layerfrac', 'stacked');
            title(figname);
            xlabel('Dynamic MT');
            ylabel('photon weight');
            numFracs = size(layerfrac, 1);
            labels = repmat({''}, 1, numFracs);
            labels{1} = '=0';
            if numFracs > 1
                labels{end} = '=1';
                edges = linspace(0, 1, numFracs - 1);
                for k = 2:numFracs - 1
                    labels{k} = sprintf('[%3.2f-%3.2f]', ...
                        edges(k - 1), edges(k));
                end
            end
            legend(labels);
            figname = sprintf('Reflected Total MT of Z, Fx = %5.3f mm', ...
                d.Fx_Midpoints(i));
            figure;
            errorbar(d.Z_Midpoints, abs(d.TotalMTOfZ(:, i)), ...
                abs(d.TotalMTOfZStdev(:, i)));
            title(figname);
            xlabel('z (mm)');
            ylabel('Total MT');
            figname = sprintf('Reflected Dynamic MT of Z, Fx = %5.3f mm', ...
                d.Fx_Midpoints(i));
            figure;
            errorbar(d.Z_Midpoints, abs(d.DynamicMTOfZ(:, i)), ...
                abs(d.DynamicMTOfZStdev(:, i)));
            title(figname);
            xlabel('z (mm)');
            ylabel('Dynamic MT');
        end
        figure;
        plot(d.SubregionIndices, d.SubregionCollisions(1, :), ...
             d.SubregionIndices, d.SubregionCollisions(2, :));
        title('Reflected Dynamic MT Of Fx: Subregion Collisions');
        xlabel('tissue region index');
        ylabel('Collisions');
        legend('static', 'dynamic');
    end

    if isfield(results{di}, 'TransmittedDynamicMTOfRhoAndSubregionHist') && ...
            show.TransmittedDynamicMTOfRhoAndSubregionHist
        d = results{di}.TransmittedDynamicMTOfRhoAndSubregionHist;
        numRhos = numel(d.Rho_Midpoints);
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.MTBins_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        xlabel('\rho [mm]');
        ylabel('Dynamic MT');
        set(gcf, 'Name', figname);
        for i = 1:20:numRhos
            figname = sprintf( ...
                'Transmitted Fractional Dynamic MT, Rho = %5.3f mm', ...
                d.Rho_Midpoints(i));
            layerfrac = squeeze(d.FractionalMT(:, :, i));
            figure;
            bar(d.MTBins_Midpoints, layerfrac', 'stacked');
            title(figname);
            xlabel('Dynamic MT');
            ylabel('photon weight');
            numFracs = size(layerfrac, 1);
            labels = repmat({''}, 1, numFracs);
            labels{1} = '=0';
            if numFracs > 1
                labels{end} = '=1';
                if numFracs > 2
                    edges = linspace(0, 1, numFracs - 1);
                    for k = 2:numFracs - 1
                        labels{k} = sprintf('[%3.2f-%3.2f]', ...
                            edges(k - 1), edges(k));
                    end
                end
            end
            legend(labels);
            figname = sprintf( ...
                'Transmitted Total MT of Z, Rho = %5.3f mm', ...
                d.Rho_Midpoints(i));
            figure;
            plot(d.Z_Midpoints, d.TotalMTOfZ(:, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Total MT');
            figname = sprintf( ...
                'Transmitted Dynamic MT of Z, Rho = %5.3f mm', ...
                d.Rho_Midpoints(i));
            figure;
            plot(d.Z_Midpoints, d.DynamicMTOfZ(:, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Dynamic MT');
        end
        figure;
        plot(d.SubregionIndices, d.SubregionCollisions(1, :), ...
             d.SubregionIndices, d.SubregionCollisions(2, :));
        title('Transmitted Dynamic MT Of Rho: Subregion Collisions');
        xlabel('tissue region index');
        ylabel('Collisions');
        legend('static', 'dynamic');
    end

    if isfield(results{di}, 'TransmittedDynamicMTOfXAndYAndSubregionHist') && ...
            show.TransmittedDynamicMTOfXAndYAndSubregionHist
        d = results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist;
        numXs = numel(d.X) - 1;
        numYs = numel(d.Y) - 1;
        yIndex = max(1, min(numYs, floor(numYs / 2)));
        xStart = max(1, min(numXs, floor(numXs / 2)));
        xIndices = xStart:min(numXs, xStart + 1);
        figname = sprintf('log10(%s) at y=0', d.Name);
        figure;
        imagesc(d.X_Midpoints, d.MTBins_Midpoints, ...
            log10(squeeze(sum(d.Mean, 2))));
        colorbar;
        title(figname);
        xlabel('x [mm]');
        ylabel('Dynamic MT');
        set(gcf, 'Name', figname);
        for i = xIndices
            figname = sprintf( ...
                'Transmitted Fractional Dynamic MT, X = %5.3f mm, Y = %5.3f mm', ...
                d.X_Midpoints(i), d.Y_Midpoints(yIndex));
            layerfrac = squeeze(d.FractionalMT(:, :, yIndex, i));
            figure;
            bar(d.MTBins_Midpoints, layerfrac', 'stacked');
            title(figname);
            xlabel('Dynamic MT');
            ylabel('photon weight');
            numFracs = size(layerfrac, 1);
            labels = repmat({''}, 1, numFracs);
            labels{1} = '=0';
            if numFracs > 1
                labels{end} = '=1';
                if numFracs > 2
                    edges = linspace(0, 1, numFracs - 1);
                    for k = 2:numFracs - 1
                        labels{k} = sprintf('[%3.2f-%3.2f]', ...
                            edges(k - 1), edges(k));
                    end
                end
            end
            legend(labels);
            figname = sprintf( ...
                'Transmitted Total MT of Z, X = %5.3f mm, Y = %5.3f mm', ...
                d.X_Midpoints(i), d.Y_Midpoints(yIndex));
            figure;
            plot(d.Z_Midpoints, d.TotalMTOfZ(:, yIndex, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Total MT');
            figname = sprintf( ...
                'Transmitted Dynamic MT of Z, X = %5.3f mm, Y = %5.3f mm', ...
                d.X_Midpoints(i), d.Y_Midpoints(yIndex));
            figure;
            plot(d.Z_Midpoints, d.DynamicMTOfZ(:, yIndex, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Dynamic MT');
        end
        figure;
        plot(d.SubregionIndices, d.SubregionCollisions(1, :), ...
             d.SubregionIndices, d.SubregionCollisions(2, :));
        title('Transmitted Dynamic MT Of X and Y: Subregion Collisions');
        xlabel('tissue region index');
        ylabel('Collisions');
        legend('static', 'dynamic');
    end

    if isfield(results{di}, 'TransmittedDynamicMTOfFxAndSubregionHist') && ...
            show.TransmittedDynamicMTOfFxAndSubregionHist
        d = results{di}.TransmittedDynamicMTOfFxAndSubregionHist;
        numFxs = numel(d.Fx);
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Fx_Midpoints, d.MTBins_Midpoints, log10(abs(d.Mean)));
        colorbar;
        title(figname);
        xlabel('Fx [/mm]');
        ylabel('Dynamic MT');
        set(gcf, 'Name', figname);
        colormap(jet);
        for i = 1:10:numFxs
            figname = sprintf( ...
                'Transmitted Fractional Dynamic MT, Fx = %5.3f mm', ...
                d.Fx_Midpoints(i));
            layerfrac = squeeze(abs(d.FractionalMT(:, :, i)));
            figure;
            bar(d.MTBins_Midpoints, layerfrac', 'stacked');
            title(figname);
            xlabel('Dynamic MT');
            ylabel('photon weight');
            numFracs = size(layerfrac, 1);
            labels = repmat({''}, 1, numFracs);
            if numFracs >= 1
                labels{1} = '=0';
            end
            if numFracs >= 2
                labels{end} = '=1';
            end
            if numFracs > 2
                edges = linspace(0, 1, numFracs - 1);
                for k = 2:numFracs - 1
                    labels{k} = sprintf('[%3.2f-%3.2f]', ...
                        edges(k - 1), edges(k));
                end
            end
            legend(labels);
            figname = sprintf( ...
                'Transmitted Total MT of Z, Fx = %5.3f mm', ...
                d.Fx_Midpoints(i));
            figure;
            errorbar(d.Z_Midpoints, abs(d.TotalMTOfZ(:, i)), ...
                abs(d.TotalMTOfZStdev(:, i)));
            title(figname);
            xlabel('z (mm)');
            ylabel('Total MT');
            figname = sprintf( ...
                'Transmitted Dynamic MT of Z, Fx = %5.3f mm', ...
                d.Fx_Midpoints(i));
            figure;
            errorbar(d.Z_Midpoints, d.DynamicMTOfZ(:, i), ...
                d.DynamicMTOfZStdev(:, i));
            title(figname);
            xlabel('z (mm)');
            ylabel('Dynamic MT');
        end
        figure;
        plot(d.SubregionIndices, d.SubregionCollisions(1, :), ...
             d.SubregionIndices, d.SubregionCollisions(2, :));
        title('Transmitted Dynamic MT Of Fx: Subregion Collisions');
        xlabel('tissue region index');
        ylabel('Collisions');
        legend('static', 'dynamic');
    end

    if isfield(results{di}, 'ReflectedTimeOfRhoAndSubregionHist') && ...
            show.ReflectedTimeOfRhoAndSubregionHist
        d = results{di}.ReflectedTimeOfRhoAndSubregionHist;
        for i = 1:numel(d.SubregionIndices)
            figname = sprintf('%s Region Index %d', d.Name, i - 1);
            figure;
            imagesc(d.Rho_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(:, i, :))'));
            colorbar;
            caxis([-15 0]);
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('time [ns]');
            xlabel('\rho [mm]');
        end
        figname = sprintf('%s Fractional Time', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Time_Midpoints, d.FractionalTime');
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('subregion index');
        xlabel('\rho [mm]');
        disp(['Time in Subregion captured by ReflectedTimeOfRhoAndSubregionHist ' ...
            'detector: ' num2str(sum(d.Mean(:)))]);
    end

    if isfield(results{di}, 'pMCATotal') && show.pMCATotal
        disp(['Total absorption captured by pMCATotal detector: ' ...
            num2str(results{di}.pMCATotal.Mean)]);
    end

    if isfield(results{di}, 'pMCROfRho') && show.pMCROfRho
        d = results{di}.pMCROfRho;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('pMC R(\rho) [mm^{-2}]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        disp(['Total reflectance captured by pMCROfRho detector: ' ...
            num2str(sum(d.Mean .* rhonorm'))]);
    end

    if isfield(results{di}, 'pMCROfRhoRecessed') && ...
            show.pMCROfRhoRecessed
        d = results{di}.pMCROfRhoRecessed;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('pMC R(\rho) [mm^{-2}]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        disp(['Total reflectance captured by pMCROfRhoRecessed detector: ' ...
            num2str(sum(d.Mean .* rhonorm'))]);
    end

    if isfield(results{di}, 'pMCROfRhoAndTime') && show.pMCROfRhoAndTime
        d = results{di}.pMCROfRhoAndTime;
        figure;
        imagesc(d.Rho_Midpoints, d.Time_Midpoints, log10(d.Mean));
        colorbar;
        title(sprintf('log10(%s)', d.Name));
        set(gcf, 'Name', d.Name);
        xlabel('\rho [mm]');
        ylabel('time [ns]');
        numtimes = numel(d.Time) - 1;
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        total = timedelta * sum(d.Mean .* repmat(rhonorm, numtimes, 1), 'all');
        disp(['Total reflectance captured by pMCROfRhoAndTime detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'pMCROfRhoAndTimeRecessed') && ...
            show.pMCROfRhoAndTimeRecessed
        d = results{di}.pMCROfRhoAndTimeRecessed;
        figure;
        imagesc(d.Rho_Midpoints, d.Time_Midpoints, log10(d.Mean));
        colorbar;
        title(sprintf('log10(%s)', d.Name));
        set(gcf, 'Name', d.Name);
        xlabel('\rho [mm]');
        ylabel('time [ns]');
        numtimes = numel(d.Time) - 1;
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        total = timedelta * sum(d.Mean .* repmat(rhonorm, numtimes, 1), 'all');
        disp(['Total reflectance captured by ROfRhoAndTimeRecessed detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'pMCROfRhoAndTimeAndSubregion') && ...
            show.pMCROfRhoAndTimeAndSubregion
        d = results{di}.pMCROfRhoAndTimeAndSubregion;
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i - 1);
            figure;
            imagesc(d.Rho_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('\rho [mm]');
            ylabel('time [ns]');
        end
        numtimes = numel(d.Time) - 1;
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        total = timedelta * sum(d.Mean .* repmat(rhonorm, [1, 1, size(d.Mean, 1)]), 'all');
        disp(['Total reflectance captured by ROfRhoAndTimeAndSubregion detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'pMCROfRhoAndTimeAndSubregionRecessed') && ...
            show.pMCROfRhoAndTimeAndSubregionRecessed
        d = results{di}.pMCROfRhoAndTimeAndSubregionRecessed;
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i - 1);
            figure;
            imagesc(d.Rho_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('time [ns]');
            xlabel('\rho [mm]');
        end
        numtimes = numel(d.Time) - 1;
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        total = timedelta * sum(d.Mean .* reshape(rhonorm, 1, [], 1), 'all');
        disp(['Total reflectance captured by ROfRhoAndTimeAndSubregionRecessed detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'dMCdROfRhodMua') && show.dMCdROfRhodMua
        d = results{di}.dMCdROfRhodMua;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('dMC dR(\rho)/dMua [mm^{-2}]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        disp(['Total reflectance captured by dMCdROfRhodMua detector: ' ...
            num2str(sum(d.Mean .* rhonorm'))]);
    end

    if isfield(results{di}, 'dMCdROfRhodMus') && show.dMCdROfRhodMus
        d = results{di}.dMCdROfRhodMus;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('dMC dR(\rho)/dMus [mm^{-2}]');
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        disp(['Total reflectance captured by dMCdROfRhodMus detector: ' ...
            num2str(sum(d.Mean .* rhonorm'))]);
    end

    if isfield(results{di}, 'dMCdROfRhoAndTimedMua') && ...
            show.dMCdROfRhoAndTimedMua
        d = results{di}.dMCdROfRhoAndTimedMua;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Time_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('time [ns]');
        xlabel('\rho [mm]');
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        total = timedelta * sum(d.Mean .* reshape(rhonorm, 1, []), 'all');
        disp(['Total reflectance captured by dMCdROfRhoAndTimedMua detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'dMCdROfRhoAndTimedMus') && ...
            show.dMCdROfRhoAndTimedMus
        d = results{di}.dMCdROfRhoAndTimedMus;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.Rho_Midpoints, d.Time_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('time [ns]');
        xlabel('\rho [mm]');
        timedelta = diff(d.Time(1:2));
        rhonorm = 2 * pi * d.Rho_Midpoints * diff(d.Rho(1:2));
        total = timedelta * sum(d.Mean .* reshape(rhonorm, 1, []), 'all');
        disp(['Total reflectance captured by dMCdROfRhoAndTimedMus detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'pMCROfXAndY') && show.pMCROfXAndY
        d = results{di}.pMCROfXAndY;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        imagesc(d.X_Midpoints, d.Y_Midpoints, log10(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        ylabel('y [mm]');
        xlabel('x [mm]');
        xdelta = diff(d.X(1:2));
        ydelta = diff(d.Y(1:2));
        total = xdelta * ydelta * sum(d.Mean, 'all');
        disp(['Total reflectance captured by pMCROfXAndY detector: ' ...
            num2str(total)]);
    end

    if isfield(results{di}, 'pMCROfXAndYAndTimeAndSubregion') && ...
            show.pMCROfXAndYAndTimeAndSubregion
        d = results{di}.pMCROfXAndYAndTimeAndSubregion;
        y0idx = floor(numel(d.Y_Midpoints) / 2);
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i);
            figure;
            imagesc(d.X_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, y0idx, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            ylabel('time [ns]');
            xlabel('x [mm]');
        end
        xdelta = diff(d.X(1:2));
        ydelta = diff(d.Y(1:2));
        timedelta = diff(d.Time(1:2));
        disp(['Total reflectance captured by pMCROfXAndYAndTimeAndSubregion detector: ' ...
            num2str(xdelta * ydelta * timedelta * sum(d.Mean, 'all'))]);
        disp(['Total reflectance captured by pMCROfXAndYAndTimeAndSubregion detector - ROfXAndY: ' ...
            num2str(xdelta * ydelta * sum(d.ROfXAndY, 'all'))]);
    end

    if isfield(results{di}, 'pMCROfXAndYAndTimeAndSubregionRecessed') && ...
            show.pMCROfXAndYAndTimeAndSubregionRecessed
        d = results{di}.pMCROfXAndYAndTimeAndSubregionRecessed;
        y0idx = floor(numel(d.Y_Midpoints) / 2);
        for i = 2:d.NumberOfRegions - 1
            figname = sprintf('log10(%s) region idx=%d', d.Name, i - 1);
            figure;
            imagesc(d.X_Midpoints, d.Time_Midpoints, ...
                log10(squeeze(d.Mean(i, :, y0idx, :))));
            colorbar;
            title(figname);
            set(gcf, 'Name', figname);
            xlabel('x [mm]');
            ylabel('time [ns]');
        end
        xdelta = diff(d.X(1:2));
        ydelta = diff(d.Y(1:2));
        timedelta = diff(d.Time(1:2));
        disp(['Total reflectance captured by ' d.Name ': ' ...
            num2str(xdelta * ydelta * timedelta * sum(d.Mean, 'all'))]);
        disp(['Total reflectance captured by ' d.Name ' - ROfXAndY: ' ...
            num2str(xdelta * ydelta * sum(d.ROfXAndY, 'all'))]);
    end

    if isfield(results{di}, 'pMCROfFx') && show.pMCROfFx
        d = results{di}.pMCROfFx;
        figname = sprintf('%s - Amplitude', d.Name);
        figure;
        plot(d.Fx_Midpoints, abs(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('f_x [/mm]');
        ylabel('R(f_x) [unitless]');
        disp(['Total reflectance captured by ' d.Name ': ' ...
            num2str(d.Amplitude(1))]);
    end

    if isfield(results{di}, 'dMCdROfFxdMua') && show.dMCdROfFxdMua
        d = results{di}.dMCdROfFxdMua;
        figname = sprintf('%s - Amplitude', d.Name);
        figure;
        plot(d.Fx_Midpoints, abs(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('f_x [/mm]');
        ylabel('dR(f_x)/dMua [unitless]');
        disp(['Total reflectance captured by ' d.Name ': ' ...
            num2str(d.Amplitude(1))]);
    end

    if isfield(results{di}, 'dMCdROfFxdMus') && show.dMCdROfFxdMus
        d = results{di}.dMCdROfFxdMus;
        figname = sprintf('%s - Amplitude', d.Name);
        figure;
        plot(d.Fx_Midpoints, abs(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('f_x [/mm]');
        ylabel('dR(f_x)/dMus [unitless]');
        disp(['Total reflectance captured by ' d.Name ': ' ...
            num2str(d.Amplitude(1))]);
    end

    if isfield(results{di}, 'pMCROfFxAndTime') && show.pMCROfFxAndTime
        d = results{di}.pMCROfFxAndTime;
        figname = sprintf('%s - Amplitude', d.Name);
        figure;
        imagesc(d.Fx_Midpoints, d.Time_Midpoints, abs(d.Mean));
        colorbar;
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('f_x [/mm]');
        ylabel('time [ns]');

        timedelta = diff(d.Time(1:2));
        disp(['Total reflectance captured by ' d.Name ': ' ...
            num2str(timedelta * d.Amplitude(1))]);
    end

    if isfield(results{di}, 'pMCTOfRho') && show.pMCTOfRho
        d = results{di}.pMCTOfRho;
        figname = sprintf('log10(%s)', d.Name);
        figure;
        plot(d.Rho_Midpoints, log10(d.Mean));
        title(figname);
        set(gcf, 'Name', figname);
        xlabel('\rho [mm]');
        ylabel('pMC T(\rho) [mm^{-2}]');
        disp(['Total reflectance captured by pMCTOfRho detector: ' ...
            num2str(sum(d.Mean, 'all'))]);
    end
  end
end
