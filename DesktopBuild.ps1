$debugbuildswitches="/p:WarningLevel=2 /nologo /v:n"
#$releasebuildswitches="/p:Configuration=Release /p:WarningLevel=2 /nologo /v:n" can't get this to work
$releasebuildswitches="/p:Configuration=Release"

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
#Write-Host "Restoring NuGet packages" -ForegroundColor Green
#Install-Package NuGetEnablePackageRestore

Write-Host "Build Vts.Desktop and Vts.Desktop.Test Debug and Release" -ForegroundColor Green
& $msbuild "$PWD\src\Vts.Desktop\Vts.Desktop.csproj" $debugbuildswitches
& $msbuild "$PWD\src\Vts.Desktop\Vts.Desktop.csproj" $releasebuildswitches

& $msbuild "$PWD\src\Vts.Desktop.Test\Vts.Desktop.Test.csproj" $debugbuildswitches
& $msbuild "$PWD\src\Vts.Desktop.Test\Vts.Desktop.Test.csproj" $releasebuildswitches


