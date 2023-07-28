$version = $args[0];
if (!$args) {
  $version="x.xx.x"
  echo "version set to x.xx.x"
}

$vtslevel = $PWD
.\BuildTestCore.ps1 

$mcclcsproj = "$PWD\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj"
$mccltestcsproj = "$PWD\src\Vts.MonteCarlo.CommandLineApplication.Test\Vts.MonteCarlo.CommandLineApplication.Test.csproj"
$mcppcsproj = "$PWD\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj"
$mcpptestcsproj = "$PWD\src\Vts.MonteCarlo.PostProcessor.Test\Vts.MonteCarlo.PostProcessor.Test.csproj"

Write-Host "Build MCCL Debug, Release" -ForegroundColor Green
dotnet build $mcclcsproj -c Debug
dotnet build $mcclcsproj -c Release
Write-Host "Test Vts.MonteCarlo.CommandLineApplication Debug and Release" -ForegroundColor Green
dotnet build $mccltestcsproj -c Debug
dotnet build $mccltestcsproj -c Release
dotnet test $mccltestcsproj -l "console;verbosity=quiet" -c Debug
dotnet test $mccltestcsproj -c Release

Write-Host "Build MCPP Debug, Release" -ForegroundColor Green
dotnet build $mcppcsproj -c Debug
dotnet build $mcppcsproj -c Release
Write-Host "Test Vts.MonteCarlo.PostProcessor Debug and Release" -ForegroundColor Green
dotnet build $mcpptestcsproj -c Debug
dotnet build $mcpptestcsproj -c Release
dotnet test $mcpptestcsproj -l "console;verbosity=quiet" -c Debug
dotnet test $mcpptestcsproj -c Release

Write-Host "Release Packages: version = $version" -ForegroundColor Green

Write-Host "Create clean publish and release folders" -ForegroundColor Green
Remove-Item "$PWD\release" -Recurse -ErrorAction Ignore
Remove-Item $PWD\publish -Recurse -ErrorAction Ignore

dotnet publish $mcclcsproj -c Release -r linux-x64 -o $PWD\publish\linux-x64 --self-contained false 
dotnet publish $mcclcsproj -c Release -r win-x64 -o $PWD\publish\win-x64 --self-contained false 
dotnet publish $mcclcsproj -c Release -r osx-x64 -o $PWD\publish\osx-x64 --self-contained false 
dotnet publish $mcclcsproj -c Release -o $PWD\publish\local --self-contained false

dotnet publish $mcppcsproj -c Release -r linux-x64 -o $PWD\publish\linux-x64 --self-contained false
dotnet publish $mcppcsproj -c Release -r win-x64 -o $PWD\publish\win-x64 --self-contained false 
dotnet publish $mcppcsproj -c Release -r osx-x64 -o $PWD\publish\osx-x64 --self-contained false 

# Create MCCL zip files for different OS
$releasedir = ".\release"
New-Item -Path $PWD -Name $releasedir -ItemType "directory"

# Create win-x64 zip
$archive = $releasedir + "\MC_v" + $version + "_Win_x64.zip"
$source = "publish\win-x64\*"
Compress-Archive -Path $source -DestinationPath $archive 
$matlabfiles = "$PWD\matlab\post_processing\*"
Compress-Archive -Path $matlabfiles -Update -DestinationPath $archive
$mcinversegeneralfiles = "$PWD\matlab\monte_carlo_inverse\general\*"
Compress-Archive -Path $mcinversegeneralfiles -Update -DestinationPath $archive
$mcinversefiles = "$PWD\matlab\monte_carlo_inverse\windows\*"
Compress-Archive -Path $mcinversefiles -Update -DestinationPath $archive

# Create linux-x64 zip
$archive = $releasedir + "\MC_v" + $version + "_Linux_x64.zip"
$source = "publish\linux-x64\*"
Compress-Archive -Path $source -DestinationPath $archive 
$matlabfiles = "$PWD\matlab\post_processing\*"
Compress-Archive -Path $matlabfiles -Update -DestinationPath $archive
$mcinversegeneralfiles = "$PWD\matlab\monte_carlo_inverse\general\*"
Compress-Archive -Path $mcinversegeneralfiles -Update -DestinationPath $archive
$mcinversefiles = "$PWD\matlab\monte_carlo_inverse\linux_and_mac\*"
Compress-Archive -Path $mcinversefiles -Update -DestinationPath $archive

# Create osx-x64 zip
$archive = $releasedir + "\MC_v" + $version + "_Mac_x64.zip"
$source = "publish\osx-x64\*"
Compress-Archive -Path $source -DestinationPath $archive 
$matlabfiles = "$PWD\matlab\post_processing\*"
Compress-Archive -Path $matlabfiles -Update -DestinationPath $archive
$mcinversegeneralfiles = "$PWD\matlab\monte_carlo_inverse\general\*"
Compress-Archive -Path $mcinversegeneralfiles -Update -DestinationPath $archive
$mcinversefiles = "$PWD\matlab\monte_carlo_inverse\linux_and_mac\*"
Compress-Archive -Path $mcinversefiles -Update -DestinationPath $archive

Write-Host "Run MCCL MATLAB post-processing tests" -ForegroundColor Green
# Change current dir to publish\local to run tests with local executable
cd "$vtslevel\publish\local"

# Generate infiles and run Monte Carlo with general infile
dotnet mc.dll geninfiles
dotnet mc.dll infile=infile_one_layer_all_detectors.txt

# Change current dir to MATLAB Monte Carlo post-processing
cd "$vtslevel\matlab\post_processing"

# remove any residual folder
# Copy results from Monte Carlo to current directory 
$MCmatlabdir = "$vtslevel\matlab\post_processing\one_layer_all_detectors"
Remove-Item  $MCmatlabdir -Recurse -ErrorAction Ignore
New-Item $MCmatlabdir -ItemType "directory"
$MCresults = "$vtslevel\publish\local\one_layer_all_detectors\*"
Copy-Item -Path $MCresults -Destination $MCmatlabdir -Recurse -ErrorAction Ignore

# run load_results_script (default datanames is set to one_layer_all_detectors) 
matlab -wait -r "load_results_script; quit"

#cleanup one_layer_all_detectors folder
Remove-Item  $MCmatlabdir -Recurse -ErrorAction Ignore

# cd back to start
cd $vtslevel

Read-Host -Prompt "Press Enter to exit MCCL build process"
