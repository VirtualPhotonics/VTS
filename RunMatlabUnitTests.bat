rem ********** RUN THIS FILE AS ADMINISTRATOR **********
set rootdir=%~dp0
set version=2.0.0
set EnableNuGetPackageRestore=true

rem ********** CHANGE CURRENT DIR TO LOCATION OF BAT FILE **********
cd %~dp0

rem ********** CHANGE CURRENT DIR TO MATLAB VTS_WRAPPER DIRECTORY **********
cd "%rootdir%matlab\vts_wrapper"

rem ********** RUN VTS_WRAPPER TESTS **********
matlab -r "vts_tests; quit" -logfile vts_wrapper_logfile

# could add code to check logfile but if anything errors in unit tests,
# the "quit" will not get invoked and matlab will not close


#rem ********** CHANGE CURRENT DIR TO BUILD DIRECTORY AND EXTRACT MC ZIP **********
#cd "%rootdir%build"

# would like to run MC with infile_one_layer_all_detectors.txt and then
# run load_results_script.m 