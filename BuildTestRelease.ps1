$version = "4.10.0"

Write-Host "Build Vts and Vts.Desktop libraries Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts\Vts.csproj -c Debug
dotnet build $PWD\src\Vts\Vts.csproj -c Release
Start-Process $PWD\DesktopBuild.bat -Wait

Write-Host "Build MCCL Debug, Release & Publish" -ForegroundColor Green
Remove-Item $PWD\publish -Recurse -ErrorAction Ignore
$mcclcsproj = "$PWD\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj"
dotnet build $mcclcsproj -c Debug
dotnet build $mcclcsproj -c Release
dotnet build $mcclcsproj -c Release -r win-x64 -o $PWD\publish\win-x64
dotnet build $mcclcsproj -c Release -r linux-x64 -o $PWD\publish\linux-x64

Write-Host "Build MCPP Debug, Release & Publish" -ForegroundColor Green
$mcppcsproj = "$PWD\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj"
dotnet build $mcppcsproj -c Debug
dotnet build $mcppcsproj -c Release
dotnet build $mcppcsproj -c Release -r win-x64 -o $PWD\publish\win-x64
dotnet build $mcppcsproj -c Release -r linux-x64 -o $PWD\publish\linux-x64

Write-Host "Build Vts.Test Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Release
Write-Host "Run Vts.Test Debug and Release" -ForegroundColor Green
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Release
Write-Host "Run Vts.Desktop.Test Debug and Release" -ForegroundColor Green
Start-Process $PWD\DesktopTests.bat -WAIT

Write-Host "Run MATLAB unit tests" -ForegroundColor Green
Invoke-Expression "& .\RunMATLABUnitTests.ps1"

Write-Host "Clean Release Folders" -ForegroundColor Green
Remove-Item "$PWD/build" -Recurse -ErrorAction Ignore
Remove-Item "$PWD/matlab/vts_wrapper/vts_libraries" -Recurse -ErrorAction Ignore
Remove-Item "$PWD/matlab/vts_wrapper/results*" -Recurse -ErrorAction Ignore

Write-Host "Create Release Packages" -ForegroundColor Green
$runtime = "win-x64"
Invoke-Expression "& .\CreateMCCLRelease.ps1 $version $runtime"
$runtime = "linux-x64"
Invoke-Expression "& .\CreateMCCLRelease.ps1 $version $runtime"
Start-Process $PWD\CreateMATLABRelease.bat $version 

Read-Host -Prompt "Press Enter to exit"
