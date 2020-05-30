Write-Host "Run vts_wrapper tests" -ForegroundColor Green
Write-Host "MATLAB not exiting nicely indicates test failure" -ForegroundColor Green
$vtslevel = $PWD

# Change current dir to vts\matlab\vts_wrapper and get rid of prior build results
cd "$vtslevel\matlab\vts_wrapper"
$matlablibdir = "$vtslevel\matlab\vts_wrapper\vts_libraries"
Remove-Item $matlablibdir -Recurse -ErrorAction Ignore
New-Item $matlablibdir -ItemType "directory"

# put MCCL and MCPP executables and supporting libraries into vts_libraries folder
$mcclcsproj = "$vtslevel\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj"
dotnet publish  $mcclcsproj -c Release -r win-x64 --self-contained false -o $matlablibdir
dotnet build $mcclcsproj -c Release -r win-x64 -o $matlablibdir
$mcppcsproj = "$vtslevel\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj"
dotnet publish $mcppcsproj -c Release -r win-x64 --self-contained false -o $matlablibdir
dotnet build $mcppcsproj -c Release -r win-x64 -o $matlablibdir

# UNCOMMENT NEXT LINE WHEN FILES IN vts_libraries
matlab -r "vts_tests; quit" 
#matlab -r "mc_tests; structures_tests; quit" # to only run unit tests

Write-Host "Run MCCL MATLAB post-processing tests" -ForegroundColor Green
# Change current dir to publish (assumes NetStandardBuildTest run prior)
cd "$vtslevel\publish\win-x64"
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
#matlab -r "load_results_script; quit"
# cd back to start
cd $vtslevel
