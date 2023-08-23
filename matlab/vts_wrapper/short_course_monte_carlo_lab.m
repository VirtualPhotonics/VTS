%% Monte Carlo Laboratory B, 
% Script for short course laboratory
%%
clear
clc

startup();

% ======================================================================= %
%% Example 01a: run Monte Carlo simulations for fluence with increasing N photons
clear;
Nphot=[10, 100, 1000, 10000]; % number of photons launched takes about 1 mins

% simulation options initiation
options = SimulationOptions();
% set method to account for absorption
options.AbsorptionWeightingType = 'Analog'; % options 'Analog' or 'Discrete'
% seed of random number generator (-1=randomly selected seed, >=0 reproducible sequence)
options.Seed = 0;

% create a point source directed perpendicularly into the tissue 
sourceInput = DirectionalPointSourceInput();
% Point source type 
sourceInput.SourceType = 'DirectionalPoint';
% New position 
sourceInput.PointLocation = [0 0 0];    
% Point source emitting direction
sourceInput.Direction = [0 0 1];  
% Initial tissue region index        
sourceInput.InitialTissueRegionIndex = 0; 

% create a single layer tissue of thickness 100mm 
tissueInput = MultiLayerTissueInput();

% assign the tissue layer regions struct
tissueInput.LayerRegions = struct(...
    'ZRange', ...
    {...
        [-Inf, 0], ... % air "z" range
        [0, 100], ... % tissue "z" range
        [100, +Inf] ... % air "z" range
    }, ...
    'RegionOP', ...
    {...
        [0.0, 1e-10, 1.0, 1.0], ... % air optical properties
        [0.01, 1.0, 0.8, 1.4], ... % tissue OPs [ua, us', g, n]
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
    } ...
);
 
% (rho,z) cylindrical coordinates grid definition in units mm
% define endpoints of rho and z tally bins and midpoints for plotting purposes
rhos = linspace(0,10,101);
rho_midpoints = (rhos(1:end-1)+rhos(2:end))/2;
zs = linspace(0,10,101);
z_midpoints = (zs(1:end-1)+zs(2:end))/2;

% create simulation inputs for increasing N
for i=1:size(Nphot,2)
  % create a default set of inputs
  si(i) = SimulationInput();
  si(i).Options = options;
  % modify number of photons
  si(i).N = Nphot(i);
  si(i).TissueInput = tissueInput;
  % specify Fluence(rho,z) detector by the endpoints of rho and z bins
  si(i).DetectorInputs = { DetectorInput.FluenceOfRhoAndZ(rhos,zs) };
  % calculate the 2nd moment of the estimates so that variance can be calculated
  si(i).DetectorInputs{1}.TallySecondMoment = true;
end
% run all simulations
output = VtsMonteCarlo.RunSimulations(si);

% plot out results of each simulation 
scrsz=get(0,'ScreenSize');
f=figure('Position',[scrsz(3)/50 scrsz(4)/10 scrsz(3)/2 scrsz(4)/2]);
set(f,'Name',sprintf('Fluence using %s absorption',options.AbsorptionWeightingType));
% spell out left and bottom corners of each plot to reduce white space
left=[0.09 0.59 0.09 0.59];bottom=[0.58 0.58 0.1 0.1];
xs = [-fliplr(rho_midpoints),rho_midpoints];
for j=1:size(Nphot,2)
  d = output{j}.Detectors(output{j}.DetectorNames{1});
  % plot fluence as a function of rho and z with mirror image
  %subplot(2,2,j,'Position',[left(j) bottom(j) 0.4 0.4]); % 2014b code
  pos=[left(j) bottom(j) 0.38 0.38]; % 2016b fix
  ax(j)=subplot(2,2,j); % 2016b fix
  set(ax(j),'Position',pos); % 2016b fix
  imagesc(xs,z_midpoints,log10([fliplr(d.Mean') d.Mean']));
  colormap(parula);
  colorbar; caxis([-6 1]); 
  shading('flat'); axis equal; axis([-9.5 9.5, 0 9.5]); 
  text(-8.5, 8, sprintf('N=%d',floor(Nphot(j))),'FontSize',16,'Color',[1 1 1]);
  title('log(Flu(\rho,z)) [mm^-^2]','FontSize',16); 
  xlabel('\rho [mm]','FontSize',16); ylabel('z [mm]','FontSize',16); 
end
f=figure('Position',[scrsz(3)/2 scrsz(4)/10 scrsz(3)/2 scrsz(4)/2]);
set(f,'Name',sprintf('Relative error using %s absorption',options.AbsorptionWeightingType));
for j=1:size(Nphot,2)
  d = output{j}.Detectors(output{j}.DetectorNames{1});
  % omit last bin since contains tallies for everything beyond last bin
  Mean = d.Mean(1:end-1,1:end-1);
  SecondMoment = d.SecondMoment(1:end-1,1:end-1);
  % determine standard deviation of the results
  SD = sqrt((SecondMoment - (Mean .* Mean)) / Nphot(j));
  relErr = SD./Mean;
  % set NaN to value beyond max
  maxval = max(relErr(:));
  relErr(isnan(relErr)) = maxval + maxval/10;
  %subplot(2,2,j,'Position',[left(j) bottom(j) 0.4 0.4]);  % 2014b code
  pos=[left(j) bottom(j) 0.38 0.38]; % 2016b fix
  ax(j)=subplot(2,2,j); % 2016b fix
  set(ax(j),'Position',pos); % 2016b fix
  imagesc(xs(1:end-2),z_midpoints(1:end-1),([fliplr(relErr') relErr'])); colormap(jet);
  % for making NaN values white
  colordata = colormap;
  colordata(end,:) = [0 0 0];
  colormap((colordata));
  colorbar;
  caxis([0 1]);
  shading('flat'); axis equal;axis([-9.5 9.5, 0 9.5]);
  text(-8.5, 8, sprintf('N=%d',floor(Nphot(j))),'FontSize',16,'Color',[1 1 0]);
  title('relerr(Flu(\rho,z))', 'FontSize',16); 
  xlabel('\rho [mm]','FontSize',16); ylabel('z [mm]','FontSize',16);
  if (strcmp(options.AbsorptionWeightingType,'Analog')==true)
    save(sprintf('analogMean%d.txt',j),'-ascii','Mean');
    save(sprintf('analogSD%d.txt',j),'-ascii','SD');
  elseif (strcmp(options.AbsorptionWeightingType,'Discrete')==true)
    save(sprintf('dawMean%d.txt',j),'-ascii','Mean');
    save(sprintf('dawSD%d.txt',j),'-ascii','SD');
  end
end

%% Example 01b: Compare standard deviation of two absorption methods
% this cell relies on above cell execution
f=figure('Position',[scrsz(3)/5 scrsz(4)/5 scrsz(3)/2 scrsz(4)/2]);
set(f,'Name','Analog relative error - DAW relative error');
for j=1:size(Nphot,2)
  analogSD = load(sprintf('analogSD%d.txt',j));
  analogMean = load(sprintf('analogMean%d.txt',j));
  dawSD = load(sprintf('dawSD%d.txt',j));
  dawMean = load(sprintf('dawMean%d.txt',j));
  analogRelErr = analogSD./analogMean;
  dawRelErr = dawSD./dawMean;
  %subplot(2,2,j,'Position',[left(j) bottom(j) 0.4 0.4]); 
  pos=[left(j) bottom(j) 0.38 0.38]; % 2016b fix
  ax(j)=subplot(2,2,j); % 2016b fix
  set(ax(j),'Position',pos); % 2016b fix
  imagesc(xs,z_midpoints,([fliplr((analogRelErr-dawRelErr)') (analogRelErr-dawRelErr)'])); 
  colordata = colormap(jet);
  colordata(1,:) = [0 0 0];
  colormap((colordata));
  colorbar; caxis([0 1]);
  shading('flat'); axis equal;axis([-9.5 9.5, 0 9.5]);
  text(-8.5, 8, sprintf('N=%d',floor(Nphot(j))),'FontSize',16,'Color',[1 1 1]);
  title('RE(analog)-RE(DAW)','FontSize',16); 
  xlabel('\rho [mm]','FontSize',16); ylabel('z [mm]','FontSize',16);
end

% ======================================================================= %
%% Example 02: run Monte Carlo simulations accounting for absorption with
% analog and continuous absorption weighting with 10,000 photons and compare
% time and relative error

% create a default set of inputs
si = SimulationInput();
si.N = 10000;
% simulation options initiation
options = SimulationOptions(); 
% seed of random number generator (-1=randomly selected seed, >=0 reproducible sequence)
options.Seed = 0;
si.Options = options;

% create a new 'instance' of the MultiLayerTissueInput class
tissueInput = MultiLayerTissueInput();
% assign the tissue layer regions struct
tissueInput.LayerRegions = struct(...
    'ZRange', ...
    {...
        [-Inf, 0], ... % air "z" range
        [0, 100], ... % tissue "z" range
        [100, +Inf] ... % air "z" range
    }, ...
    'RegionOP', ...
    {...
        [0.0, 1e-10, 1.0, 1.0], ... % air optical properties
        [0.1, 1, 0.8, 1.4], ... % tissue OPs [ua, us', g, n]
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
    } ...
);
si.TissueInput = tissueInput;

% specify a single R(rho) detector by the endpoints of rho bins
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,10,101)) };
si.DetectorInputs{1}.TallySecondMoment = true;

% specify analog absorption and run simulation
si.Options.AbsorptionWeightingType = 'Analog';
disp('Analog results:');
output1 = VtsMonteCarlo.RunSimulation(si);

% specify continuous absorption weighting (CAW) and run simulation
si.Options.AbsorptionWeightingType = 'Continuous';
disp('Continuous absorption weighting results:');
output2 = VtsMonteCarlo.RunSimulation(si);

d1 = output1.Detectors(output1.DetectorNames{1});
d2 = output2.Detectors(output2.DetectorNames{1});
% determine standard deviation using 1st and 2nd moment results
d1SD = sqrt((d1.SecondMoment - (d1.Mean .* d1.Mean)) / si.N);
d2SD = sqrt((d2.SecondMoment - (d2.Mean .* d2.Mean)) / si.N);
% plot R(rho) using both methods with errorbars
f=figure; 
set(f,'Name','Spatially-resolved reflectance with 1-sigma error bars');
subplot(2,1,1);
errorbar(d1.Rho, d1.Mean, d1SD,'r-');
hold on;
errorbar(d2.Rho, d2.Mean, d2SD,'b-');
set(gca,'YScale','log');
axis([0 9.9, 1e-6 1]);
title('log(R(\rho)) [mm^-^2]'); %xlabel('Rho (mm)');
l=legend('analog','CAW');
set(l,'FontSize',10);

% plot analog relative error - CAW relative error
subplot(2,1,2);
plot(d1.Rho, (d1SD./d1.Mean-d2SD./d2.Mean),'g-',d1.Rho,zeros(length(d1.Rho),1),'k:');
%axis([0 9.9, -0.1 0.1]);
title('analog relative error - CAW relative error'); xlabel('Rho (mm)');