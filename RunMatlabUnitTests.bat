rem ********** RUN THIS FILE AS ADMINISTRATOR **********
set rootdir=%~dp0

rem ********** CHANGE CURRENT DIR TO MATLAB VTS_WRAPPER DIRECTORY **********
cd "%rootdir%matlab\vts_wrapper"

rem ********** RUN VTS_WRAPPER TESTS **********
rem ********** MATLAB NOT EXITING NICELY INDICATES TEST FAILURE **********
matlab -r "vts_tests; quit" 
#matlab -r "mc_tests; structures_tests; quit" # to only run unit tests


rem ********** CHANGE CURRENT DIR TO VTS.MONTECARLO.COMMANDLINEAPPLICATION/BIN/RELEASE DIRECTORY  **********
cd "%rootdir%src\Vts.MonteCarlo.CommandLineApplication\bin\Release"

rem ********** RUN MONTE CARLO WITH GENERAL INFILE  **********
mc.exe infile=infile_one_layer_all_detectors.txt

rem ********** CHANGE CURRENT DIR TO MATLAB MONTE CARLO POST-PROCESSING DIRECTORY **********
cd "%rootdir%matlab\post_processing\monte_carlo\simulation_result_loading"

rem ********** COPY RESULTS FROM MONTE CARLO TO CURRENT DIRECTORY **********
# remove any residual folder
rmdir "%rootdir%matlab\post_processing\monte_carlo\simulation_result_loading\one_layer_all_detectors" /s /q 
xcopy "%rootdir%src\Vts.MonteCarlo.CommandLineApplication\bin\Release\one_layer_all_detectors\*" "%rootdir%matlab\post_processing\monte_carlo\simulation_result_loading\one_layer_all_detectors\"

rem ********** RUN LOAD_RESULTS_SCRIPT (DEFAULT DATANAMES IS SET TO one_layer_all_detectors) **********
rem ********** MATLAB NOT EXITING NICELY INDICATES TEST FAILURE **********
matlab -r "load_results_script; quit"
