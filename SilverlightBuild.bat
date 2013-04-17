set rootdir=%~dp0
set debugbuildswitches=/p:WarningLevel=2 /nologo /v:n
set releasebuildswitches=/p:Configuration=Release /p:WarningLevel=2 /nologo /v:n
set releaseWLbuildswitches=/p:Configuration=ReleaseWhiteList /p:WarningLevel=2 /nologo /v:n
set EnableNuGetPackageRestore=true

set msbuild=%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild

"%msbuild%" "%rootdir%\src\Vts\Vts.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts\Vts.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.Gui.Silverlight\Vts.Gui.Silverlight.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.Gui.Silverlight\Vts.Gui.Silverlight.csproj" %releasebuildswitches%
"%msbuild%" "%rootdir%\src\Vts.Gui.Silverlight\Vts.Gui.Silverlight.csproj" %releaseWLbuildswitches%

"%msbuild%" "%rootdir%\src\Vts.Test\Vts.Test.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.Test\Vts.Test.csproj" %releasebuildswitches%

rem pause