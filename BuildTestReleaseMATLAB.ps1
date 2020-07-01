$version = $args[0];
if (!$args) {
  $version="x.xx.x"
  echo "version set to x.xx.x"
}

Write-Host "Build Vts.Desktop library Debug & Release" -ForegroundColor Green
$debugbuildswitches='/p:WarningLevel=2','/nologo','/v:n'
$releasebuildswitches='/p:Configuration=Release','/p:WarningLevel=2','/nologo','/v:n' 

$msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe"
if (-not (Test-Path "$msbuild")) {
  $msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
}
if (-not (Test-Path "$msbuild")) {
  $msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
}
if (-not (Test-Path "$msbuild")) {
  $msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
}
if (-not (Test-Path "$msbuild")) { 
Write-Host "***** Please install the prerequisites to build VTS *****"
pause
exit
}
Write-Host "$msbuild" -ForegroundColor Green
Write-Host "Build Vts.Desktop and Vts.Desktop.Test Debug and Release" -ForegroundColor Green
& $msbuild "$PWD\src\Vts.Desktop\Vts.Desktop.csproj" $debugbuildswitches
& $msbuild "$PWD\src\Vts.Desktop\Vts.Desktop.csproj" $releasebuildswitches

& $msbuild "$PWD\src\Vts.Desktop.Test\Vts.Desktop.Test.csproj" $debugbuildswitches
& $msbuild "$PWD\src\Vts.Desktop.Test\Vts.Desktop.Test.csproj" $releasebuildswitches

Write-Host "Test Vts.Desktop.Test Debug and Release" -ForegroundColor Green
$nunit="$PWD\tools\nunit\Desktop\nunit3-console"

& "$nunit" "/result:TestResult-Vts-Debug.xml" "$PWD\src\Vts.Desktop.Test\bin\Debug\Vts.Desktop.Test.dll" 
& "$nunit" "/result:TestResult-Vts-Release.xml" "$PWD\src\Vts.Desktop.Test\bin\Release\Vts.Desktop.Test.dll"

Write-Host "Release Packages" -ForegroundColor Green
Write-Host "Clean Release folders" -ForegroundColor Green
Remove-Item "$PWD/matlab/vts_wrapper/vts_libraries" -Recurse -ErrorAction Ignore
Remove-Item "$PWD/matlab/vts_wrapper/results*" -Recurse -ErrorAction Ignore

Write-Host "Test MATLAB unit tests" -ForegroundColor Green
# RunMATLABUnitTests copies Vts.Desktop/bin/Release files to matlab/vts_wrapper/vts_libraries
.\RunMATLABUnitTests.ps1 

Write-Host "Create MATLAB Release version = $version" -ForegroundColor Green
$archive="$PWD\release\VTS_MATLAB_v" + $version + "Beta.zip"
$source="$PWD\matlab\vts_wrapper\*"

Compress-Archive -Path $source -DestinationPath $archive

Read-Host -Prompt "Press Enter to exit MATLAB release process"
