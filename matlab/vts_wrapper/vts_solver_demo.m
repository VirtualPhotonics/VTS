%% Solver Demos
% Script for demoing the use of the VTS solvers within Matlab, to view the
% source code open the *vts_solver_demo.m* script file.
%% 
clear vars
clc
dbstop if error;

startup();

%% Example 01: ROfRhoAndFt
% Evaluate reflectance as a function of rho and temporal-frequency with one 
% set of optical properites.

op = [0.01 1.2 0.8 1.4];
rho = 10; % s-d separation, in mm
ft = 0:0.01:0.5; % range of temporal frequencies in GHz

% Solver type options: 'PointSourceSDA','DistributedGaussianSourceSDA',
% 'DistributedPointSourceSDA','MonteCarlo' (basic scaled),'Nurbs' (scaled
% with smoothing and adaptive binning)
VtsSolvers.SetSolverType('PointSourceSDA');

test = VtsSolvers.ROfRhoAndFt(op, rho, ft);

f = figure;
set(f, 'OuterPosition', [100, 50, 600, 800]);
subplot(3,1,1); plot(ft, [real(test) -imag(test)] );
title('Reflectance vs f_t'); 
ylabel('R(f_t)');
xlabel('f_t');
lgnd = legend('Real','Imaginary');
set(lgnd, 'FontSize', 12);
subplot(3,1,2); plot(ft, abs(test));
title('Reflectance amplitude vs f_t'); 
ylabel('R(f_t) Amplitude');
xlabel('f_t');
subplot(3,1,3); plot(ft, -angle(test));% or 'plot(ft, atan2(imag(test),real(test)))'
title('Reflectance phase vs f_t'); 
ylabel('R(f_t) Phase');
xlabel('f_t');
set(f,'Name','Reflectance as a function of Rho and Temporal-frequency');

%% Example 02: ROfFxAndFt
% Evaluate reflectance as a function of spatial- and temporal- frequencies 
% with one set of optical properites.

op = [0.01 1 0.8 1.4];
fx = 0; % spatial frequency in 1/mm
ft = linspace(0,0.5,51); % range of temporal frequencies in GHz

test = VtsSolvers.ROfFxAndFt(op, fx, ft);

f = figure;
set(f, 'OuterPosition', [100, 50, 600, 800]);
subplot(3,1,1); plot(ft, [real(test) -imag(test)] );
title('Reflectance vs f_t'); 
ylabel('R(f_t)');
xlabel('f_t');
legend('Real','Imaginary');
subplot(3,1,2); plot(ft, abs(test));
title('Reflectance amplitude vs f_t'); 
ylabel('R(f_t) Amplitude');
xlabel('f_t');
subplot(3,1,3); plot(ft, -angle(test));% or 'plot(ft, atan2(imag(test),real(test)))'
title('Reflectance phase vs f_t'); 
ylabel('R(f_t) Phase');
xlabel('f_t');
set(f,'Name','Reflectance as a function of Spatial- and Temporal- frequencies');

%% Example 03: FluenceOfRhoAndZ
% Evaluate fluence as a function of rho and z using optical properties from 
% a list of chromophore absorbers with their concentrations and a power law 
% scatterer for a range of wavelengths.

rhos = 0.1:0.1:10; % s-d separation, in mm
zs = 0.1:0.1:10; % z range in mm

wv = 450:0.5:1000;

% create a list of chromophore absorbers and their concentrations
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations =  [70,     30,   0.8  ];

% create a scatterer (PowerLaw, Intralipid, or Mie)
scatterer.Type = 'PowerLaw';
scatterer.A = 1.2;
scatterer.b = 1.42;

% % or 
% scatterer.Type = 'Intralipid';
% scatterer.vol_frac =  0.5;

% % or 
% scatterer.Type = 'Mie';
% scatterer.radius =  0.5;
% scatterer.n =       1.4;
% scatterer.nMedium = 1.0;
% scatterer.vol_frac = 0.5;

op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);

test = VtsSolvers.FluenceOfRhoAndZ(op, rhos, zs);

f = figure; imagesc(wv,zs,log(squeeze(test(:,1,:))));
ylabel('z [mm]');
xlabel('wavelength, \lambda [nm]');
title('Fluence of \lambda and z at \rho=0.1 mm'); 
set(f,'Name','Fluence as a function of lambda and z');


%% Example 04: FluenceOfRhoAndZAndFt
% Evaluate fluence as a function of rho and z using one set of optical 
% properties and a distributed gaussian source SDA solver type.

op = [0.01 1 0.8 1.4];
rhos = linspace(0.1,19.9,100); % s-d separation, in mm
zs = linspace(0.1,19.9,100); % z range in mm
fts = linspace(0,1,2); % frequency range in GHz

VtsSolvers.SetSolverType('DistributedPointSourceSDA');
test = VtsSolvers.FluenceOfRhoAndZAndFt(op, rhos, zs, fts);

xs = [-fliplr(rhos(2:end)),rhos];
% xs = [-rhos(end:-1:2), rhos];

% f = figure; imagesc(log(test));
f = figure; imagesc(xs,zs,log([fliplr(squeeze(test(1,:,2:end))),squeeze(test(1,:,:))]));
axis image
title('Fluence of \rho and z and ft (ft=0GHz)'); 
xlabel('\rho [mm]')
ylabel('z [mm]')
set(f,'Name','Fluence of Rho and z and ft (ft=0GHz)');

f = figure; imagesc(xs,zs,log([fliplr(squeeze(test(2,:,2:end))),squeeze(test(2,:,:))]));
axis image
title('Fluence of \rho and z and ft (ft=1GHz)'); 
xlabel('\rho [mm]')
ylabel('z [mm]')
set(f,'Name','Fluence of Rho and z and ft (ft=1GHz)');

% 2nd figure to show modulation
modulation = squeeze(test(2,:,:)./test(1,:,:));
f = figure; imagesc(xs,zs,[fliplr(modulation(:,2:end)), modulation]);
axis image
title('Modulation of fluence (AC/DC) of \rho & z & ft (ft=1GHz)'); 
xlabel('\rho [mm]')
ylabel('z [mm]')
set(f,'Name','Modulation of fluence (AC/DC) of Rho and z and ft (ft=1GHz)');

%% Example 05: PHDOfRhoAndZ
% Evaluate Photon Hitting Density in cylindrical coordinates
% using one set of optical properties and a distributed gaussian source SDA
% solver type.

op = [0.01 1 0.8 1.4];
rhos = linspace(0.1,19.9,100); % s-d separation, in mm
zs = linspace(0.1,19.9,100); % z range in mm

VtsSolvers.SetSolverType('DistributedGaussianSourceSDA');
test = VtsSolvers.PHDOfRhoAndZ(op, rhos, zs, 10);

f = figure; imagesc(rhos, zs, log(test));
axis image;
title('Photon Hitting Density of \rho and z'); 
xlabel('\rho [mm]')
ylabel('z [mm]')
set(f,'Name','PHD of Rho and z');

%% Example 06: FluenceOfRhoAndZTwoLayer
% Evaluate fluence in cylindrical coordinates for a two
% layer tissue with specified source-detector separation and top layer thickness
% using two sets of optical properties.

op = [[1 1 0.8 1.4];[0.1 1 0.8 1.4]];
rhos = linspace(0.1,19.9,100); % s-d separation, in mm
zs = linspace(0.1,19.9,100); % z range in mm
topLayerThickness=5; % mm
test = VtsSolvers.FluenceOfRhoAndZTwoLayer(op, rhos, zs, topLayerThickness);

f = figure; imagesc(rhos, zs, log(test));
axis image;
title('Fluence of \rho and z - Two Layer'); 
xlabel('\rho [mm]');
ylabel('z [mm]');
set(f,'Name','Fluence of Rho and z - Two Layer');

%% Example 07: PHDOfRhoAndZTwoLayer
% Evaluate Photon Hitting Density in cylindrical coordinates for a two
% layer tissue with specified source-detector separation and top layer thickness
% using two sets of optical properties.

op = [[0.01 1 0.8 1.4];[0.1 1 0.8 1.4]];
rhos = linspace(0.1,19.9,100); % s-d separation, in mm
zs = linspace(0.1,19.9,100); % z range in mm
sdsep = 10; % mm
topLayerThickness=5; % mm
test = VtsSolvers.PHDOfRhoAndZTwoLayer(op, rhos, zs, sdsep, topLayerThickness);

f = figure; imagesc(rhos, zs, log(test));
axis image;
title('Photon Hitting Density of \rho and z - Two Layer'); 
xlabel('\rho [mm]');
ylabel('z [mm]');
set(f,'Name','PHD of Rho and z - Two Layer');

%% Example 08: AbsorbedEnergyOfRhoAndZ
% Evaluate Absorbed Energy of rho and z using one set of optical properties 
% and a point source SDA solver type.

op = [0.1 1 0.8 1.4];
rhos = linspace(0.1,19.9,100); % s-d separation, in mm
zs = linspace(0.1,19.9,100); % z range in mm

VtsSolvers.SetSolverType('PointSourceSDA');

xs = [-fliplr(rhos(2:end)),rhos];

test = VtsSolvers.AbsorbedEnergyOfRhoAndZ(op, rhos, zs);
% f = figure; imagesc(log(test));
f = figure; imagesc(xs, zs, log([fliplr(test),test]));
axis image;
title('Absorbed Energy of \rho and z'); 
xlabel('\rho [mm]');
ylabel('z [mm]');
set(f,'Name','Absorbed Energy of Rho and z');

%% Example 09: ROfRho
% Evaluate reflectance as a function of rho with three sets
% of optical properites.

op = [[0.01 1 0.8 1.4]; [0.1 1 0.8 1.4]; [1 1 0.8 1.4]];
rho = 0.5:0.05:9.5; %s-d separation, in mm
%VtsSolvers.SolverType = 'DistributedPointSourceSDA';
test = VtsSolvers.ROfRho(op, rho);
f = figure; plot(rho, test);
set(f,'Name','Reflectance vs Rho for various optical properties');
% create the legend with just the mua value from the optical properties
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(op(:,1), '\mu_a: ', 'mm^-^1', options);
title('Reflectance vs \rho for various optical properties'); 
ylabel('R(\rho)');
xlabel('\rho');

%% Example 10: ROfRhoAndTime
% Evaluate reflectance of rho and t at one s-d separation and 
% two sets of optical properites.

op = [[0.1 1.2 0.8 1.4]; [0.2 1.2 0.8 1.4]];
rho = 10; %s-d separation, in mm
t = 0:0.001:0.5; % range of times in ns
test0 = VtsSolvers.ROfRhoAndT(op, rho, t);
f = figure; plot(t, squeeze(test0));
set(f,'Name','Reflectance of Rho vs time for various optical properties');
% create the legend with just the mua value from the optical properties
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(op(:,1), '\mu_a: ', 'mm^-^1', options);
title({'Reflectance of \rho vs time for various optical properties'; ' '}); 
ylabel('R(t)');
xlabel('Time, t [ns]');

%% Example 11: ROfFxAndTime
% Evaluate reflectance as a function of spacial-frequency and t using one 
% set of optical properites.

op = [0.1 1 0.8 1.4];
fx = linspace(0,0.5,11); % range of spatial frequencies in 1/mm
t = linspace(0,0.05,501); % range of times in ns
test = VtsSolvers.ROfFxAndT(op, fx, t);
f = figure; plot(t,squeeze(test(:,:,:)));
set(f,'Name','R of fx and t (Reflectance vs time)');
set(f, 'OuterPosition', [100, 50, 700, 700]);
title('Reflectance vs time at various spatial frequencies'); 
ylabel('R(f_x, t)');
xlabel('Time, t [ns]');
%Create the legend dynamically
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(fx, 'f_x = ', 'mm^-^1', options);
t = [0.01 0.05];
test = VtsSolvers.ROfFxAndT(op, fx, t);
f = figure; plot(fx,squeeze(test));
set(f,'Name','R of fx and t (Reflectance vs spatial frequency)');
title('Reflectance vs spatial frequency at 0.01 and 0.05 ns'); 
ylabel('R(f_x, t)');
xlabel('Spatial frequency, f_x [mm^-^1]');
%Create the legend dynamically - can be replaced by PlotHelper.CreateLegend(t, 't = ', 'ns', '');
l2 = cell(1,length(t)); %create a 1xlength(t) cell array to hold the string values
for i=1:length(t)
    str = sprintf('t = %2.2fns', t(i));
    l2{1,i} = str;
end
legend(l2, 'FontSize', 12);

%% Example 12: ROfFx (single set of optical properties)
% Evaluate reflectance as a function of spacial-frequency with a single set 
% of optical properties.

fx = 0:0.001:0.2; % range of spatial frequencies in 1/mm
op = [0.1 1.2 0.8 1.4];
test1 = VtsSolvers.ROfFx(op, fx);
f = figure; plot(fx, test1);
set(f,'Name','R of fx');
title('Reflectance vs spatial frequency'); 
ylabel('R(f_x)');
xlabel('Spatial frequency, f_x [mm^-^1]');

%% Example 13: ROfFx (multiple sets of optical properties)
% Evaluate reflectance as a function of spacial-frequency
% with multiple sets of optical properties, varying mua linearly.

fx = 0:0.001:0.2; % range of spatial frequencies in 1/mm
mua = (0:0.01:0.1)';
op = repmat([0 1.2 0.8 1.4],[length(mua) 1]);
op(:,1) = mua;

test2 = VtsSolvers.ROfFx(op, fx);
f = figure; plot(fx, test2);
set(f, 'Name', 'R of fx (varying mua linearly)');
set(f, 'OuterPosition', [100, 50, 700, 700]);
title('Reflectance vs spatial frequency'); 
options = [{'Location', 'NorthEast'}; {'FontSize', 12}; {'Box', 'on'}];
PlotHelper.CreateLegend(op(:,1), '\mu_a: ', 'mm^-^1', options);
ylabel('R(f_x)');
xlabel('Spatial frequency, f_x [mm^-^1]');

%% Example 14: ROfFx (multiple optical properties, varying mua as a function of wavelength, mus' as intralipid scatterer)
% Evaluate reflectance as a function of spacial-frequency with multiple 
% sets of optical properties, varying mua as a function of wavelength.

fx = 0:0.05:0.2; % range of spatial frequencies in 1/mm
wv = 450:0.5:1000;
nwv = length(wv);

% create a list of chromophore absorbers and their concentrations
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations =  [70,     30,   0.8  ];

% create a scatterer (PowerLaw, Intralipid, or Mie)
% scatterer.Type = 'PowerLaw';
% scatterer.A = 1.2;
% scatterer.b = 1.42;

% % or 
scatterer.Type = 'Intralipid';
scatterer.vol_frac =  0.01;

% % or 
% scatterer.Type = 'Mie';
% scatterer.radius =  0.5;
% scatterer.n =       1.4;
% scatterer.nMedium = 1.0;
% scatterer.vol_frac = 0.001;

op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);

% plot the absorption spectrum
f = figure; plot(wv, op(:,1));
set(f, 'Name', 'R of fx (plot absorption spectrum)');
title('Absorption vs wavelength');
ylabel('\mu_a(\lambda)');
xlabel('Wavelength, \lambda [nm]');

% plot the log of the absorption spectrum
f = figure; semilogy(wv, op(:,1));
set(f, 'Name', 'R of fx (plot log of the absorption spectrum)');
title('log(Absorption) vs wavelength'); 
ylabel('\mu_a(\lambda)');
xlabel('Wavelength, \lambda [nm]');

% plot the scattering spectrum
f = figure; plot(wv, op(:,2));
set(f, 'Name', 'R of fx (plot scattering spectrum)');
title('Reduced scattering vs wavelength'); 
ylabel('\mu_s^''(\lambda)');
xlabel('Wavelength, \lambda [nm]');

% calculate and plot the resulting reflectance spectrum at each frequency
test3 = VtsSolvers.ROfFx(op, fx);
f = figure; plot(wv, test3);
set(f, 'Name', 'R of fx (plot resulting reflectance spectrum at each frequency)');
title('SFD Reflectance vs wavelength'); 
ylabel('R(\lambda)');
xlabel('Wavelength, \lambda [nm]');
options = [{'Location', 'NorthWest'}; {'FontSize', 12}; {'Box', 'on'}];
PlotHelper.CreateLegend(fx, 'f_x = ', 'mm^-^1', options);

%% Example 15: ROfFx (multiple optical properties, varying mua as a function of wavelength, mus' as mie scatterer)
% Evaluate reflectance as a function of spacial-frequency with multiple 
% sets of optical properties, varying mua as a function of wavelength.

fx = 0:0.05:0.2; % range of spatial frequencies in 1/mm
wv = 450:0.5:1000;
nwv = length(wv);

% create a list of chromophore absorbers and their concentrations
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations =  [70,     30,   0.8  ];

% create a scatterer (PowerLaw, Intralipid, or Mie)
% scatterer.Type = 'PowerLaw';
% scatterer.A = 1.2;
% scatterer.b = 1.42;

% % or 
% scatterer.Type = 'Intralipid';
% scatterer.vol_frac =  0.01;

% % or 
scatterer.Type = 'Mie';
scatterer.radius =  0.5;
scatterer.n =       1.4;
scatterer.nMedium = 1.0;
scatterer.vol_frac = 0.001;

op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);

% plot the absorption spectrum
f = figure; plot(wv, op(:,1));
set(f, 'Name', 'R of fx (plot absorption spectrum)');
title('Absorption vs wavelength');
ylabel('\mu_a(\lambda)');
xlabel('Wavelength, \lambda [nm]');

% plot the log of the absorption spectrum
f = figure; semilogy(wv, op(:,1));
set(f, 'Name', 'R of fx (plot log of the absorption spectrum)');
title('log(Absorption) vs wavelength'); 
ylabel('\mu_a(\lambda)');
xlabel('Wavelength, \lambda [nm]');

% plot the scattering spectrum
f = figure; plot(wv, op(:,2));
set(f, 'Name', 'R of fx (plot scattering spectrum)');
title('Reduced scattering vs wavelength'); 
ylabel('\mu_s^''(\lambda)');
xlabel('Wavelength, \lambda [nm]');

% calculate and plot the resulting reflectance spectrum at each frequency
test3 = VtsSolvers.ROfFx(op, fx);
f = figure; plot(wv, test3);
set(f, 'Name', 'R of fx (plot resulting reflectance spectrum at each frequency)');
title('SFD Reflectance vs wavelength'); 
ylabel('R(\lambda)');
xlabel('Wavelength, \lambda [nm]');
options = [{'Location', 'NorthWest'}; {'FontSize', 12}; {'Box', 'on'}];
PlotHelper.CreateLegend(fx, 'f_x = ', 'mm^-^1', options);

%% Example 16: ROfFx
% Call planar reflectance with multiple sets of optical
% properties, varying the scattering prefactor as a function of wavelength.

fx = 0; % spatial frequency in 1/mm
wv = 400:0.5:1000;

% create a list of chromophore absorbers and their concentrations
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations =  [70,     30,   0.8  ];

% create a scatterer (PowerLaw, Intralipid, or Mie)
scatterer.Type = 'PowerLaw';
scatterer.A = 1.0;
scatterer.b = 1.42;

A = 0.5:0.25:2.5;
test4 = zeros([length(A) length(wv)]);
for i=1:length(A)
    scatterer.A = A(i);
    op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
    test4(i,:) = VtsSolvers.ROfFx(op, fx);
end

f = figure; plot(wv, test4');
set(f, 'Name', 'Planar reflectance');
set(f, 'OuterPosition', [100, 50, 800, 800]);
title('SFD Reflectance vs wavelength'); 
ylabel('R(\lambda)');
xlabel('Wavelength, \lambda [nm]');
options = [{'Location', 'NorthWest'}; {'FontSize', 12}; {'Box', 'on'}];
PlotHelper.CreateLegend(A, '\mu_s''(1000nm) = ', 'mm^-^1', options);

%% Example 17: ROfRho (multiple wavelengths, multiple rho)
% Call reflectance varying the wavelength.

VtsSolvers.SetSolverType('PointSourceSDA');
rho = 0.2:0.2:1;  % source-detector separation in mm
wv = 400:0.5:1000;

% create a list of chromophore absorbers and their concentrations
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations =  [70,     30,   0.8  ];

% create a scatterer (PowerLaw, Intralipid, or Mie)
scatterer.Type = 'PowerLaw';
scatterer.A = 1.2;
scatterer.b = 1.42;

op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
test = zeros([length(rho) length(wv)]);
for i=1:length(rho)
    test(i,:) = VtsSolvers.ROfRho(op, rho(i));
end

f = figure; plot(wv, test');
set(f, 'Name', 'SDA Reflectance vs wavelength');
set(f, 'OuterPosition', [100, 50, 800, 800]);
title('SDA Reflectance vs wavelength'); 
ylabel('R(\lambda)');
xlabel('Wavelength, \lambda [nm]');
options = [{'Location', 'NorthEast'}; {'FontSize', 12}; {'Box', 'on'}];
PlotHelper.CreateLegend(rho,'\rho = ', 'mm',options);
%% Example 18: ROfRho (inverse solution for chromophore concentrations for multiple wavelengths, single rho)

rho = 1;  % source-detector separation in mm
wv = 400:50:1000;

% create a list of chromophore absorbers and their concentrations
% these values are the EXACT solution
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
measConc =                  [70,     30,   0.8  ];
absorbers.Concentrations =  [measConc(1), measConc(2), measConc(3)];

% create a scatterer (PowerLaw, Intralipid, or Mie)
scatterer.Type = 'PowerLaw';
scatterer.A = 1.2;
scatterer.b = 1.42;

op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);

% create simulated measured data at EXACT data using MC Nurbs solution
VtsSolvers.SetSolverType('Nurbs');
measData = VtsSolvers.ROfRho(op, rho);

% Set up options for lsqcurvefit/fminsearch function

% use unconstrained optimization, constrained option lb=[0 0]; ub=[inf inf];
lb=[]; ub=[];
% specify initial guess 
conc0 = [ 70,    30,   0.8  ];
% run inverse solver using SDAPointSource forward model
if(exist('lsqcurvefit','file'))
    options = optimset('diagnostics','on','largescale','on');
    recoveredConc = lsqcurvefit('sda_F',conc0,wv,measData,lb,ub,options,rho,scatterer);
else
    options = [];
    recoveredConc = fminsearch('sda_Chi2',conc0,options,wv,rho,scatterer,measData);
end
    % determine forward solver solution at recovered concentrations
VtsSolvers.SetSolverType('PointSourceSDA');
absorbers.Concentrations =  [recoveredConc(1), recoveredConc(2), recoveredConc(3) ];
op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
recovered = VtsSolvers.ROfRho(op, rho);
% determine forward solver solution at initial guess
absorbers.Concentrations =  [conc0(1), conc0(2), conc0(3)];
op = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
initialGuess = VtsSolvers.ROfRho(op, rho);

f = figure; plot(wv, measData,'ro',...
    wv, initialGuess,'go',...
    wv, recovered,'b:');
xlabel('Wavelength, \lambda [nm]');
ylabel('R(\lambda)');
legend('Meas','IG','Converged');
set(f, 'Name', 'ROfRho (inverse solution for chromophore concentrations, multiple wavelengths, single Rho)');
title('ROfRho (inverse solution for chromophore concentrations, multiple wavelengths, single \rho)'); 
set(f, 'OuterPosition', [100, 50, 960, 850]); 
options = [{'Location', 'NorthEast'}; {'FontSize', 12}; {'Box', 'on'}];
disp(sprintf('Meas =    [%5.3f %5.3f %5.3f]',measConc(1),measConc(2),measConc(3)));
disp(sprintf('IG =      [%5.3f %5.3f %5.3f] Chi2=%5.3e',conc0(1),conc0(2),conc0(3),...
    (measData-initialGuess)*(measData-initialGuess)'));
disp(sprintf('Conv =    [%5.3f %5.3f %5.3f] Chi2=%5.3e',recoveredConc(1),recoveredConc(2),recoveredConc(3),...
    (measData-recovered)*(measData-recovered)'));
disp(sprintf('error =   [%5.3f %5.3f %5.3f]',abs(measData(1)-recovered(1))/measData(1),...
    abs(measData(2)-recovered(2))/measData(2),abs(measData(3)-recovered(3))/measData(3)));

%% Example 19: ROfRho for a two-layer tissue (multiple optical properties and rhos)
clear op
topLayerThickness = 2;  % units: mm

opsA = [0.01 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 1
opsB = [0.02 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 2
opsC = [0.03 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 3
op(1,:,:) = [opsA];
op(2,:,:) = [opsB];
op(3,:,:) = [opsC];

rho = 0.5:0.05:9.5; %s-d separation, in mm
test = VtsSolvers.ROfRhoTwoLayer(op, topLayerThickness, rho);
f = figure; semilogy(rho, test);
set(f,'Name','2-Layer Reflectance vs Rho for various top Layer Optical Properties');
% create the legend with just the mua value from the top layer optical properties
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(op(:,1), 'top \mu_a: ', 'mm^-^1', options);
title('2-Layer Reflectance vs \rho for various top Layer OPs'); 
ylabel('R(\rho)');
xlabel('\rho');

%% Example 20: ROfFx for a two-layer tissue (multiple optical properties and fxs)
clear op
topLayerThickness = 2;  % units: mm

opsA = [0.01 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 1
opsB = [0.02 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 2
opsC = [0.03 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 3
op(1,:,:) = [opsA];
op(2,:,:) = [opsB];
op(3,:,:) = [opsC];

fx = 0:0.01:0.5; % range of spatial frequencies in 1/mm
test = VtsSolvers.ROfFxTwoLayer(op, topLayerThickness, fx);
f = figure; plot(fx, test);
set(f,'Name','2-Layer Reflectance vs Fx for various top Layer Optical Properties');
% create the legend with just the mua value from the top layer optical properties
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(op(:,1), 'top \mu_a: ', 'mm^-^1', options);
title('2-Layer Reflectance vs fx for various top Layer OPs'); 
ylabel('R(fx)');
xlabel('fx');

%% Example 21: ROfRhoAndTime for a two-layer tissue (multiple optical properties and times)
clear op
topLayerThickness = 2;  % units: mm
% 
% opsA = [0.01 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 1
% opsB = [0.02 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 2
% opsC = [0.03 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 3
% op(1,:,:) = [opsA];
% op(2,:,:) = [opsB];
% op(3,:,:) = [opsC];

wv = 650:100:850;

% create a list of chromophore absorbers and their concentrations
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
absorbers.Concentrations =  [70,     30,   0.8  ];

% create a scatterer (PowerLaw, Intralipid, or Mie)
scatterer.Type = 'PowerLaw';
scatterer.A = 1.2;
scatterer.b = 1.42;

opBottomLayer = VtsSpectroscopy.GetOP(absorbers, scatterer, wv);
% get OPs at first wavelength and perturb top layer mua by factor 1.1
opsA = [opBottomLayer(1,1) opBottomLayer(1,2) opBottomLayer(1,3) opBottomLayer(1,4);
      1.1*opBottomLayer(1,1) opBottomLayer(1,2) opBottomLayer(1,3) opBottomLayer(1,4)];
% get OPs at first wavelength and perturb top layer mua
opsB = [opBottomLayer(2,1) opBottomLayer(2,2) opBottomLayer(2,3) opBottomLayer(2,4);
      1.1*opBottomLayer(2,1) opBottomLayer(2,2) opBottomLayer(2,3) opBottomLayer(2,4)];
% get OPs at first wavelength and perturb top layer mua
opsC = [opBottomLayer(3,1) opBottomLayer(3,2) opBottomLayer(3,3) opBottomLayer(3,4);
      1.1*opBottomLayer(3,1) opBottomLayer(3,2) opBottomLayer(3,3) opBottomLayer(3,4)];
op(1,:,:) = [opsA];
op(2,:,:) = [opsB];
op(3,:,:) = [opsC];
  
rho = 10; %s-d separation, in mm
t = 0:0.001:0.5; % range of times in ns
test = VtsSolvers.ROfRhoAndTimeTwoLayer(op, topLayerThickness, rho, t);
f = figure; plot(t, squeeze(test));
set(f,'Name','2-Layer Reflectance vs time for various top Layer Optical Properties');
% create the legend with just the mua value from the optical properties
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
%PlotHelper.CreateLegend(op(:,1), 'top \mu_a: ', 'mm^-^1', options);
PlotHelper.CreateLegend(wv(1,:), '', 'nm', options);
title('2-Layer Reflectance vs time for various top Layer OPs'); 
ylabel('R(t)');
xlabel('Time, t [ns]');

%% Example 22: ROfRhoAndFt for a two-layer tissue (multiple optical properties and fts)
% Evaluate reflectance as a function of rho and temporal-frequency with one 
% set of optical properites.
clear op
topLayerThickness = 2;  % units: mm

opsA = [0.01 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 1
opsB = [0.02 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 2
opsC = [0.03 1 0.8 1.4; 0.01 1 0.8 1.4]; % top/bottom layer OPs case 3
op(1,:,:) = [opsA];
op(2,:,:) = [opsB];
op(3,:,:) = [opsC];

rho = 10; % s-d separation, in mm
ft = 0:0.01:0.5; % range of temporal frequencies in GHz

test = VtsSolvers.ROfRhoAndFtTwoLayer(op, topLayerThickness, rho, ft);

f = figure;
set(f, 'OuterPosition', [100, 50, 600, 800]);
subplot(3,1,1); 
plot(ft, [real(squeeze(test(1,:,:))) -imag(squeeze(test(1,:,:)))],...
     ft, [real(squeeze(test(2,:,:))) -imag(squeeze(test(2,:,:)))],...
     ft, [real(squeeze(test(3,:,:))) -imag(squeeze(test(3,:,:)))]);
title('2-Layer Reflectance vs f_t'); 
ylabel('R(f_t)');
xlabel('f_t');
lgnd = legend('Real','Imaginary');
set(lgnd,'FontSize',12);
subplot(3,1,2); 
plot(ft, abs(squeeze(test(1,:,:))),...
     ft, abs(squeeze(test(2,:,:))),...
     ft, abs(squeeze(test(3,:,:))));
title('2-Layer Reflectance amplitude vs f_t'); 
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(op(:,1), 'top \mu_a: ', 'mm^-^1', options);
ylabel('R(f_t) Amplitude');
xlabel('f_t');
subplot(3,1,3); 
plot(ft, -angle(squeeze(test(1,:,:))),...
     ft, -angle(squeeze(test(2,:,:))),...
     ft, -angle(squeeze(test(3,:,:))));
title('2-Layer Reflectance phase vs f_t'); 
ylabel('R(f_t) Phase');
xlabel('f_t');
set(f,'Name','Frequency-domain reflectance');
options = [{'FontSize', 12}; {'Location', 'NorthEast'}];
PlotHelper.CreateLegend(op(:,1), 'top \mu_a: ', 'mm^-^1', options);