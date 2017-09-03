%% VTS MATLAB Tests
% Script for running all unit tests within Matlab
% Set runDemosToo to 'true' to run the demos and the unit tests
clear
clc
dbstop if error;
startup();
runDemosToo = true;

% mc_tests(); % CKH comment out 9/2/17 matlab/mccl interop code not working
structures_tests();

if(runDemosToo)    
    vts_solver_demo();    
    %vts_mc_demo(); % CKH comment out 9/2/17 matlab/mccl interop not working
end