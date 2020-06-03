$version = "4.10.0"

Write-Host "Build Vts and Vts.Desktop libraries Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts\Vts.csproj -c Debug
dotnet build $PWD\src\Vts\Vts.csproj -c Release
.\DesktopBuild.ps1 -wait

Write-Host "Build MCCL Debug, Release" -ForegroundColor Green
$mcclcsproj = "$PWD\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj"
dotnet build $mcclcsproj -c Debug
dotnet build $mcclcsproj -c Release

Write-Host "Build MCPP Debug, Release" -ForegroundColor Green
$mcppcsproj = "$PWD\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj"
dotnet build $mcppcsproj -c Debug
dotnet build $mcppcsproj -c Release

Write-Host "Build Vts.Test Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Release
Write-Host "Run Vts.Test Debug and Release" -ForegroundColor Green
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Release

Write-Host "Test Vts.Desktop.Test Debug and Release" -ForegroundColor Green
.\DesktopTests.ps1 -wait > $null

Write-Host "Release Packages" -ForegroundColor Green
Write-Host "Clean Release folders" -ForegroundColor Green
Remove-Item "$PWD/build" -Recurse -ErrorAction Ignore
Remove-Item "$PWD/matlab/vts_wrapper/vts_libraries" -Recurse -ErrorAction Ignore
Remove-Item "$PWD/matlab/vts_wrapper/results*" -Recurse -ErrorAction Ignore
if (Test-Path $PWD\publish) {
  Remove-Item $PWD\publish -Recurse -ErrorAction Ignore
}
New-Item -Path $PWD -Name ".\publish\win-x64" -ItemType "directory"

# run next 2 line prior to RunMATLABUnitTests to setup up publish with results
dotnet build $mcclcsproj -c Release -r win-x64 -o $PWD\publish\win-x64 
dotnet build $mcppcsproj -c Release -r win-x64 -o $PWD\publish\win-x64 

Write-Host "Test MATLAB unit tests" -ForegroundColor Green
# RunMATLABUnitTests copies Vts.Desktop/bin/Release files to matlab/vts_wrapper/vts_libraries
.\RunMATLABUnitTests.ps1 

$runtime = "win-x64"
Invoke-Expression ".\CreateMCCLRelease.ps1 $version $runtime"
Invoke-Expression ".\CreateMATLABRelease.ps1 $version"

Read-Host -Prompt "Press Enter to exit"
