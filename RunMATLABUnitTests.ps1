Write-Host "Run vts_wrapper tests" -ForegroundColor Green
Write-Host "MATLAB not exiting nicely indicates test failure" -ForegroundColor Green
$vtslevel = $PWD

# Change current dir to vts\matlab\vts_wrapper and get rid of prior build results
cd "$vtslevel\matlab\vts_wrapper"
$matlablibdir = "$vtslevel\matlab\vts_wrapper\vts_libraries"
Remove-Item $matlablibdir -Recurse -ErrorAction Ignore
New-Item $matlablibdir -ItemType "directory"

# put supporting libraries into vts_libraries folder
$vtsdesktop = "$vtslevel\src\Vts.Desktop\bin\Release"
Copy-Item -Path "$vtsdesktop\*" -Destination "$matlablibdir"

# UNCOMMENT NEXT LINE WHEN FILES IN vts_libraries
matlab -wait -r "vts_tests; quit" 
#matlab -r "mc_tests; structures_tests; quit" # to only run unit tests

Write-Host "Run MCCL MATLAB post-processing tests" -ForegroundColor Green
# setup up publish with results
$mcclcsproj = "$vtslevel\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj"
dotnet build $mcclcsproj -c Release -r win-x64 -o $vtslevel\publish\win-x64 
$mcppcsproj = "$vtslevel\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj"
dotnet build $mcppcsproj -c Release -r win-x64 -o $vtslevel\publish\win-x64 

# Change current dir to publish (assumes NetStandardBuildTest run prior)
cd "$vtslevel\publish\win-x64"
$PWD
# Generate infiles and run Monte Carlo with general infile
.\mc.exe geninfiles
.\mc.exe infile=infile_one_layer_all_detectors.txt

# Change current dir to MATLAB Monte Carlo post-processing
cd "$vtslevel\matlab\post_processing\monte_carlo\simulation_result_loading"

# remove any residual folder
# Copy results from Monte Carlo to current directory 
$MCmatlabdir = "$vtslevel\matlab\post_processing\monte_carlo\simulation_result_loading\one_layer_all_detectors"
Remove-Item  $MCmatlabdir -Recurse -ErrorAction Ignore
New-Item $MCmatlabdir -ItemType "directory"
$MCresults = "$vtslevel\publish\win-x64\one_layer_all_detectors\*"
Copy-Item -Path $MCresults -Destination $MCmatlabdir -Recurse -ErrorAction Ignore

# run load_results_script (default datanames is set to one_layer_all_detectors) 
matlab -wait -r "load_results_script; quit"
# cd back to start
cd $vtslevel
