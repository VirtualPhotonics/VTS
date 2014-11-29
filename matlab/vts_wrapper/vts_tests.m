%% VTS MATLAB Tests
% Script for running all unit tests within Matlab
% Set runDemosToo to 'true' to run the demos and the unit tests
clear
clc
dbstop if error;
startup();
runDemosToo = true;

mc_tests();
structures_tests();

if(runDemosToo)    
    vts_solver_demo();    
    vts_mc_demo();
end