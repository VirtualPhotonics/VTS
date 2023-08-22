%% Monte Carlo Demo
% Script for demoing use of VTS Monte Carlo tools within Matlab, to view
% the source code see vts_mc_demo.m
%%
clear all
clc

startup();

% ======================================================================= %
%% Example 01: run a simple Monte Carlo simulation with 1000 photons

% create a default set of inputs
si = SimulationInput();

% modify number of photons
si.N = 1000;

% specify a single R(rho) detector by the endpoints of rho bins
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output = VtsMonteCarlo.RunSimulation(si);

% more work to do on making outputs friendly, but it's working :)
d = output.Detectors(output.DetectorNames{1});
f = figure; semilogy(d.Rho, d.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
title('Reflectance vs \rho for N=1000');
set(f,'Name','Reflectance vs Rho for N=1000');

% ======================================================================= %
%% Example 02: run Monte Carlo simulations for two absorption weighting types 
% with 1000 photons each and compare computation time

% create a default set of inputs
si = SimulationInput();

% specify a single R(rho) detector by the endpoints of rho bins
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };

si.Options.AbsorptionWeightingType = 'Continuous';

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output1 = VtsMonteCarlo.RunSimulation(si);

si.Options.AbsorptionWeightingType = 'Discrete';

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output2 = VtsMonteCarlo.RunSimulation(si);

% ======================================================================= %
%% Example 03: run a Monte Carlo simulation with a fully-customized input
% (values used here are the class defaults)

% 1) define a source...

% create a new 'instance' of the DirectionalPointSourceInput class
sourceInput = DirectionalPointSourceInput();
% Point source type 
sourceInput.SourceType = 'DirectionalPoint'; % dc - this shouldn't be necesary...look at detector inputs
% New position 
sourceInput.PointLocation = [0 0 0];    
% Point source emitting direction
sourceInput.Direction = [0 0 1];  
% Initial tissue region index        
sourceInput.InitialTissueRegionIndex = 0;

% 2) define a tissue...

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
        [0.0, 1.0, 0.8, 1.4], ... % tissue optical properties
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
        } ...
    ); 

% 3) specify one or more detector geometries to tally...

detectorInputs = {...
    DetectorInput.ROfRho(linspace(0,40,201))... % specifies endpoints of rho bins
};

% 4) set all options...

% creates a new 'instance' of the SimulationOptions class
options = SimulationOptions(); 
% seed of random number generator (-1=randomly selected seed, >=0 reproducible sequence)
options.Seed = -1;
% random number generator type
options.RandomNumberGeneratorType = 'MersenneTwister';
% absorption weighting type
options.AbsorptionWeightingType = 'Discrete';
% phase function type
options.PhaseFunctionType  = 'HenyeyGreenstein';
% list of databases to be written
options.Databases = {};
% flag indicating whether to track statistics about where photon ends up
options.TrackStatistics = true;
% photon weight threshold to perform Russian Roulette.  Default = 0 means no RR performed.
options.RussianRouletteWeightThreshold = 0;
% simulation index 
options.SimulationIndex = 0;

% finally, create a new 'instance' of the SimulationInput class
input = SimulationInput();
% number of photons
input.N = 100;
% name of output folder (if being written to file)
input.OutputName = 'results';

% assign source, tissue, and detector above to our input class
input.SourceInput = sourceInput;
input.TissueInput = tissueInput;
input.DetectorInputs = detectorInputs;
input.Options = options;

output = VtsMonteCarlo.RunSimulation(input);

% ======================================================================= %
%% Example 04: run a list of Monte Carlo simulations
% create a list of two default SimulationInput with different numbers of 
% photons

si1 = SimulationInput();
% modify number of photons
si1.N = 1000;

si2 = SimulationInput();
s12.N = 100;

% specify a single R(rho) detector by the endpoints of rho bins
si1.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
si2.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
% create list of these 2 imput
si = [ si1; si2 ];
% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output = VtsMonteCarlo.RunSimulations(si);
d1 = output{1}.Detectors(output{1}.DetectorNames{1});
f = figure; semilogy(d1.Rho, d1.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
title('Simulation 1 results'); 
set(f,'Name','Simulation 1 results');
d2 = output{2}.Detectors(output{2}.DetectorNames{1});
f = figure; semilogy(d2.Rho, d2.Mean); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
title('Simulation 2 results');
set(f,'Name','Simulation 2 results');

% ======================================================================= %
% Example 05: run a Monte Carlo simulation with post-processing enabled
% First run a simulation, then post-process the generated database and
% compare on-the-fly results with post-processed results
si = SimulationInput();
si.N = 1000;
% output folder name
si.OutputName = 'results';
% modify database generation to specifying creating reflectance database
options = SimulationOptions();
options.Databases = { Vts.MonteCarlo.DatabaseType.DiffuseReflectance };
si.Options = options;
% specify a single R(rho) detector by the endpoints of rho bins
si.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) };
% default writes database to "results" folder
output = VtsMonteCarlo.RunSimulation(si);
d1 = output.Detectors(output.DetectorNames{1});
% specify post-processing of generated database 
ppi = PostProcessorInput();
% read from output folder
ppi.InputFolder = si.OutputName;
ppi.DetectorInputs = { DetectorInput.ROfRho(linspace(0,40,201)) } ;
% run post-processor with exact detector as specified in simulation
ppoutput = VtsMonteCarlo.RunPostProcessor(ppi,si);
d2 = ppoutput.Detectors(ppoutput.DetectorNames{1});
% plot results and view that they are identical
f = figure; semilogy(d1.Rho, d1.Mean, 'r-',d2.Rho, d2.Mean,'g:'); ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
title('Post processing a database'); 
set(f,'Name','Post processing a database');
legend('on-the-fly','post-processed');

% ======================================================================= %
% Example 06: run a Monte Carlo simulation with pMC post-processing enabled
% First run a simulation, then post-process the generated database CA
% varying optical properties
si = SimulationInput();
si.N = 1000;
% output folder name
si.OutputName = 'results';
options = SimulationOptions();
options.AbsorptionWeightingType = 'Continuous';
% modify database generation to specifying creating pMC reflectance database
options.Databases = { Vts.MonteCarlo.DatabaseType.pMCDiffuseReflectance };
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
        [0.01, 1.0, 0.8, 1.4], ... % tissue optical properties
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
        } ...
    );
si.Options = options;
si.TissueInput = tissueInput;
output = VtsMonteCarlo.RunSimulation(si);
% specify post-processing of generated database 
ppi = PostProcessorInput();
% specify detector based on baseline infile tissue optical properties
di = DetectorInput.pMCROfRho(linspace(0,40,201));
di.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [0.01,   1.0, 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
di.PerturbedRegionsIndices = [ 1 ];
% specify detector with perturbed mus = 0.5xbaseline
di0p5xmua = DetectorInput.pMCROfRho(linspace(0,40,201),'pMCROfRho_0p5xmua');
di0p5xmua.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [0.005,   1.0, 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
di0p5xmua.PerturbedRegionsIndices = [ 1 ]; 
% specify detector with perturbed mus = 2xbaseline
di2xmua = DetectorInput.pMCROfRho(linspace(0,40,201),'pMCROfRho_2xmua');
di2xmua.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [0.02,   1.0, 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
di2xmua.PerturbedRegionsIndices = [ 1 ];
ppi.InputFolder = si.OutputName;
ppi.DetectorInputs = { di, di0p5xmua, di2xmua} ;
ppoutput = VtsMonteCarlo.RunPostProcessor(ppi,si);
do = ppoutput.Detectors(ppoutput.DetectorNames{1});
do0p5xmua = ppoutput.Detectors(ppoutput.DetectorNames{2});
do2xmua = ppoutput.Detectors(ppoutput.DetectorNames{3});
f = figure; semilogy(do.Rho, do.Mean, 'r-',do0p5xmua.Rho, do0p5xmua.Mean,'g-', do2xmua.Rho, do2xmua.Mean, 'b-'); 
ylabel('log(R(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
legend('baseline','0.5x mua','2x mua','Location','SouthWest');
title('Perturbation MC using CAW'); 
set(f,'Name','Perturbation MC using CAW');

% ======================================================================= %
% Example 07: run a Monte Carlo simulation with pMC post-processing enabled
% Use generated database to solve inverse problem with measured data
% generated using Nurbs
% NOTE: convergence to measured data optical properties affected by:
% 1) number of photons launched in baseline simulation, N
% 2) placement and number of rho
% 3) distance of initial guess from actual
% 4) normalization of chi2
% 5) optimset options selected
%% create input to simulation
si = SimulationInput();
si.N = 1000; % need to run 1e5 or greater to get decent results
% output folder name
si.OutputName = 'results';
simOptions = SimulationOptions();
simOptions.Seed = 0;
simOptions.AbsorptionWeightingType = 'Discrete';
% modify database generation to specifying creating pMC reflectance database
simOptions.Databases = { Vts.MonteCarlo.DatabaseType.pMCDiffuseReflectance };
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
        [0.01, 1.0, 0.8, 1.4], ... % tissue optical properties
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
        } ...
    );
si.Options = simOptions;
si.TissueInput = tissueInput;
%% create database
output = VtsMonteCarlo.RunSimulation(si);
%% Send optimization function that post-processes database for pMC/dMC results
% specify initial guess equal to baseline value [mua mus] NOTE: mus not mus'
muaBaseline=si.TissueInput.LayerRegions(2).RegionOP(1);
musBaseline=si.TissueInput.LayerRegions(2).RegionOP(2)/...
    (1-si.TissueInput.LayerRegions(2).RegionOP(3));
x0 = [muaBaseline, musBaseline]; 
% use unconstrained optimization, constrained option lb=[0 0]; ub=[inf inf];
lb=[]; ub=[];
% create measData using nurbs with changed optical properties
measOPs = [0.04 0.95 0.8 1.4]; % NOTE 2nd element is mus' not mus
measMus = measOPs(2)/(1-0.8);
VtsSolvers.SetSolverType('Nurbs');
% specify rho bins for pMC/dMC processing and rhoMidpoints for nurbs
rho = linspace(0,6,7);
rhoMidpoints = (rho(1:end-1) + rho(2:end))/2;
measData = VtsSolvers.ROfRho(measOPs, rhoMidpoints)';
% option: divide measured data and forward model by measured data
% this counters log decay of data and relative importance of small rho data
% NOTE: if use option here, need to use option in pmc_F_dmc_J.m 
% measDataNorm = measData./measData;

% run lsqcurvefit if have Optimization Toolbox because it makes use of
% dMC differential Monte Carlo predictions
% if don't have Optimization Toolbox, run non-gradient, non-constrained
% fminsearch
if(exist('lsqcurvefit','file'))
    options = optimset('Jacobian','on','diagnostics','on','largescale','on');
    recoveredOPs = lsqcurvefit('pmc_F_dmc_J',x0,rhoMidpoints,measData,lb,ub,...
        options,si,measData);
else
%     options = optimset('diagnostics','on','largescale','on');
    options = [];
    recoveredOPs = fminsearch('pmc_Chi2',x0,options,rhoMidpoints,si,measData);
end
% determine forward data at initial guess = background optical properties
diInitialGuess = DetectorInput.pMCROfRho(rho,'pMCROfRho_initial_guess');
diInitialGuess.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [x0(1), x0(2)*(1-0.8), 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
diInitialGuess.PerturbedRegionsIndices = [ 1 ];
% determine forward results at converged optical properties
diRecovered = DetectorInput.pMCROfRho(rho,'pMCROfRho_recovered');
diRecovered.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [recoveredOPs(1), recoveredOPs(2)*(1-0.8), 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
diRecovered.PerturbedRegionsIndices = [ 1 ];
ppi = PostProcessorInput();
ppi.InputFolder = si.OutputName;
ppi.DetectorInputs = { diInitialGuess diRecovered } ;
ppoutput = VtsMonteCarlo.RunPostProcessor(ppi,si); 
doInitialGuess = ppoutput.Detectors(ppoutput.DetectorNames{1});
doRecovered = ppoutput.Detectors(ppoutput.DetectorNames{2});
f = figure; semilogy(rhoMidpoints,measData,'ro',...
    rhoMidpoints,doInitialGuess.Mean,'g-',...
    rhoMidpoints,doRecovered.Mean,'b:');
xlabel('\rho [mm]');
ylabel('log10(R(\rho))');
legend('Meas','IG','Converged','Location','SouthWest');
title('Inverse solution using pMC/dMC'); 
set(f, 'Name', 'Inverse solution using pMC/dMC');
disp(sprintf('Meas =    [%f %5.3f]',measOPs(1),measOPs(2)/(1-0.8)));
disp(sprintf('IG =      [%f %5.3f] Chi2=%5.3e',x0(1),x0(2),...
    (measData-doInitialGuess.Mean')*(measData-doInitialGuess.Mean')'));
disp(sprintf('Conv =    [%f %5.3f] Chi2=%5.3e',recoveredOPs(1),recoveredOPs(2),...
    (measData-doRecovered.Mean')*(measData-doRecovered.Mean')'));
disp(sprintf('error =   [%f %5.3f]',abs(measOPs(1)-recoveredOPs(1))/measOPs(1),...
    abs(measMus-recoveredOPs(2))/measOPs(2)));

% ======================================================================= %
% Example 08: run a Monte Carlo simulation and verify results with
% those in unit tests in Visual Studio
% spell out all input to ensure same settings as in unit test
si = SimulationInput();
si.N = 100;
% output folder name
si.OutputName = 'results';
options = SimulationOptions();
options.AbsorptionWeightingType = 'Analog';
options.Seed = 0;
si.Options = options;
sourceInput = DirectionalPointSourceInput();
sourceInput.SourceType = 'DirectionalPoint'; 
sourceInput.PointLocation = [0 0 0];  
sourceInput.Direction = [0 0 1];       
sourceInput.InitialTissueRegionIndex = 1;
si.SourceInput = sourceInput;
tissueInput = MultiLayerTissueInput();
tissueInput.LayerRegions = struct(...
    'ZRange', ...
    {...
        [-Inf, 0], ... % air "z" range
        [0, 20], ... % tissue "z" range
        [20, +Inf] ... % air "z" range
    }, ...
    'RegionOP', ...
    {...
        [0.0, 1e-10, 1.0, 1.0], ... % air optical properties
        [0.01, 1.0, 0.8, 1.4], ... % tissue optical properties
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
        } ...
    )
si.TissueInput = tissueInput;
si.DetectorInputs =  { ...
    %DetectorInput.RDiffuse(),...
    DetectorInput.ROfRho(linspace(0,10,101)),...
    DetectorInput.ROfRhoAndOmega(linspace(0,10,101),linspace(0.05,1,101)), ...
    DetectorInput.ROfRhoAndTime(linspace(0,10,101), linspace(0,1,101))};
output = VtsMonteCarlo.RunSimulation(si);
d1 = output.Detectors(output.DetectorNames{1});
d2 = output.Detectors(output.DetectorNames{2});
d3 = output.Detectors(output.DetectorNames{3});
if ((abs(d1.Mean(1,1)-0.95492965855)<0.00000000001) && ...
    (abs(d2.Mean(1,1)-(0.95492885-i*0.000817329)<0.00000001)) && ...
    (abs(d3.Mean(1,1)-95.492965855)<0.000000001))
  disp('unit tests pass');
end

% ======================================================================= %
%% Example 09: run a Monte Carlo simulation for transmittance tallies with 1000 photons

% create a default set of inputs
si = SimulationInput();

% modify number of photons
si.N = 1000;

tissueInput = MultiLayerTissueInput(); % define a thin tissue layer
tissueInput.LayerRegions = struct(...
    'ZRange', ...
    {...
        [-Inf, 0], ... % air "z" range
        [0, 2], ... % tissue "z" range
        [2, +Inf] ... % air "z" range
    }, ...
    'RegionOP', ...
    {...
        [0.0, 1e-10, 1.0, 1.0], ... % air optical properties
        [0.01, 1.0, 0.8, 1.4], ... % tissue optical properties
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
        } ...
    )
si.TissueInput = tissueInput;

% specify a single R(rho) detector by the endpoints of rho bins
si.DetectorInputs = { DetectorInput.TOfRho(linspace(0,40,201)),...
                      DetectorInput.TOfFx(linspace(0,0.2,51)) };

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output = VtsMonteCarlo.RunSimulation(si);

d1 = output.Detectors(output.DetectorNames{1}); % TOfRho
f = figure; semilogy(d1.Rho, d1.Mean); ylabel('log(T(\rho)) [mm^-^2]'); xlabel('Rho (mm)');
title('Transmittance vs \rho for N=1000');
set(f,'Name','Transmittance vs Rho for N=1000');
d2 = output.Detectors(output.DetectorNames{2}); % TOfFx
f = figure; plot(d2.Fx, abs(d2.Mean)); ylabel('T(fx) [unitless]'); xlabel('Fx (mm^-^1)');
title('Transmittance vs fx for N=1000');
set(f,'Name','Transmittance vs Fx for N=1000');

% ======================================================================= %
%% Example 10: run R(fx) detector results

% create a default set of inputs
si = SimulationInput();

% modify number of photons
si.N = 1000;

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
        [0.01, 1.0, 0.8, 1.4], ... % tissue optical properties
        [0.0, 1e-10, 1.0, 1.0] ... % air optical properties
        } ...
    ); 
si.TissueInput = tissueInput;

% specify a single R(rho) detector by the endpoints of rho bins
si.DetectorInputs = { DetectorInput.ROfFx(linspace(0,0.5,51)) };

% use this to run a Matlab-wrapped MonteCarloSimulation using static method
output = VtsMonteCarlo.RunSimulation(si);

% more work to do on making outputs friendly, but it's working :)
d = output.Detectors(output.DetectorNames{1});
f = figure; plot(d.Fx, abs(d.Mean)); ylabel('R(fx)'); xlabel('fx (/mm)');
title('Amplitude vs fx for N=1000');
set(f,'Name','Amplitude vs Fx for N=1000');
