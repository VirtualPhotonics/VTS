% script for demoing use of Vts within Matlab
clear all
clc
dbstop if error;
startup();
runDemosToo = false;

structures_tests();
mc_tests();

if(runDemosToo)    
    vts_solver_demo();    
    vts_mc_demo();
end