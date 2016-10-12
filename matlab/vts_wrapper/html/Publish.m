%% publish the main documentation
opt.outputDir = 'html';
publish('getting_started', opt);
publish('user_guide', opt);
publish('demo', opt);
publish('function_categories', opt);

%% publish the demo documentation
opt.outputDir = 'html\demo';
opt.showCode = false;
opt.createThumbnail = false;
publish('vts_mc_demo', opt);
% need to reset opt because the monte carlo demo clears all values
opt.outputDir = 'html\demo';
opt.showCode = false;
opt.createThumbnail = false;
publish('vts_solver_demo', opt);
% publish the plots from the short course lab
opt.outputDir = 'html\demo';
opt.showCode = false;
opt.createThumbnail = false;
publish('short_course_monte_carlo_lab', opt);

%% publish the solver documentation
opt.outputDir = 'html\solvers';
publish('VtsSolvers_help', opt);
publish('AbsorbedEnergyOfRhoAndZ_help', opt);
publish('FluenceOfRhoAndZ_help', opt);
publish('PHDOfRhoAndZ_help', opt);
publish('PHDOfRhoAndZTwoLayer_help', opt);
publish('ROfFx_help', opt);
publish('ROfFxAndFt_help', opt);
publish('ROfFxAndT_help', opt);
publish('ROfFxTwoLayer_help', opt);
publish('ROfRho_help', opt);
publish('ROfRhoAndFt_help', opt);
publish('ROfRhoAndFtTwoLayer_help', opt);
publish('ROfRhoAndT_help', opt);
publish('ROfRhoAndTimeTwoLayer_help', opt);
publish('ROfRhoTwoLayer_help', opt);
publish('SetSolverType_help', opt);

%% publish the monte carlo documentation
opt.outputDir = 'html/monte_carlo';
publish('VtsMonteCarlo_help', opt);
publish('RunPostProcessor_help', opt);
publish('RunPostProcessors_help', opt);
publish('RunSimulation_help', opt);
publish('RunSimulations_help', opt);

%% publish the spectroscopy documentation
opt.outputDir = 'html/spectroscopy';
publish('VtsSpectroscopy_help', opt);
publish('GetOP_help', opt);
