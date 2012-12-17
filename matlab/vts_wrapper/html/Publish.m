%% publish the main documentation
opt.outputDir = 'html';
publish('getting_started', opt);
publish('user_guide', opt);

%% publish the demo documentation
opt.outputDir = 'html\demo';
publish('vts_mc_demo', opt);
% need to reset opt because the monte carlo demo clears all values
opt.outputDir = 'html\demo';
publish('vts_solver_demo', opt);

%% publish the solver documentation
opt.outputDir = 'html\solvers';
publish('VtsSolvers_help', opt);
publish('FluenceOfRhoAndZ_help', opt);
publish('PHDOfRhoAndZ_help', opt);
publish('ROfFx_help', opt);
publish('ROfFxAndFt_help', opt);
publish('ROfFxAndT_help', opt);
publish('ROfRho_help', opt);
publish('ROfRhoAndFt_help', opt);
publish('ROfRhoAndT_help', opt);

%% publish the monte carlo documentation
opt.outputDir = 'html/monte_carlo';
publish('VtsMonteCarlo_help', opt);

%% publish the spectroscopy documentation
opt.outputDir = 'html/spectroscopy';
publish('VtsSpectroscopy_help', opt);
