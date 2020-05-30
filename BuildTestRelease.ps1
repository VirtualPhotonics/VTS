Write-Host "Build Vts library Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts\Vts.csproj -c Debug
dotnet build $PWD\src\Vts\Vts.csproj -c Release

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

Write-Host "Create Release Packages" -ForegroundColor Green
Remove-Item $PWD\build -Recurse -ErrorAction Ignore
$version = "4.10.0"
$runtime = "win-x64"
Invoke-Expression "& .\CreateMCCLRelease.ps1 $version $runtime"
Invoke-Expression "& .\CreateMATLABRelease.ps1 $version $runtime"
$runtime = "linux-x64"
Invoke-Expression "& .\CreateMCCLRelease.ps1 $version $runtime"

Write-Host "Build Vts.Test Debug & Release" -ForegroundColor Green
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet build $PWD\src\Vts.Test\Vts.Test.csproj -c Release

Write-Host "Begin Testing Debug and Release" -ForegroundColor Green
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Debug
dotnet test $PWD\src\Vts.Test\Vts.Test.csproj -c Release

Write-Host "Run MATLAB unit tests"
Invoke-Expression "& .\RunMatlabUnitTests.ps1"

Read-Host -Prompt "Press Enter to exit"
