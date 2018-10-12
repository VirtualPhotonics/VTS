@echo off
set rootdir=%~dp0
set debugbuildswitches=/p:WarningLevel=2 /nologo /v:n
set releasebuildswitches=/p:Configuration=Release /p:WarningLevel=2 /nologo /v:n
set msbuild=%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe
if not exist "%msbuild%" set msbuild=%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe
if not exist "%msbuild%" set msbuild=%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe
if not exist "%msbuild%" set msbuild=%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe
if not exist "%msbuild%" ( 
echo *************** Please install the prerequisites to build VTS ***************
pause
exit
)
echo %msbuild%
set EnableNuGetPackageRestore=true

"%msbuild%" "%rootdir%\src\Vts.Desktop\Vts.Desktop.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.Desktop\Vts.Desktop.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.MGRTE.ConsoleApp\Vts.MGRTE.ConsoleApp.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.MGRTE.ConsoleApp\Vts.MGRTE.ConsoleApp.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.CommandLineApplication\Vts.MonteCarlo.CommandLineApplication.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.PostProcessor\Vts.MonteCarlo.PostProcessor.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.Desktop.Test\Vts.Desktop.Test.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.Desktop.Test\Vts.Desktop.Test.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.CommandLineApplication.Test\Vts.MonteCarlo.CommandLineApplication.Test.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.CommandLineApplication.Test\Vts.MonteCarlo.CommandLineApplication.Test.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.PostProcessor.Test\Vts.MonteCarlo.PostProcessor.Test.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.PostProcessor.Test\Vts.MonteCarlo.PostProcessor.Test.csproj" %releasebuildswitches%

rem pause
