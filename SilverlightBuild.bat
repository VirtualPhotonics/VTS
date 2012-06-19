set rootdir=%~dp0
set debugbuildswitches=/p:WarningLevel=2 /nologo /v:n
set releasebuildswitches=/p:Configuration=Release /p:WarningLevel=2 /nologo /v:n
set releaseWLbuildswitches=/p:Configuration=ReleaseWhiteList /p:WarningLevel=2 /nologo /v:n

set msbuild=%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild

"%msbuild%" "%rootdir%\src\Vts\Vts.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts\Vts.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.SiteVisit\Vts.SiteVisit.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.SiteVisit\Vts.SiteVisit.csproj" %releasebuildswitches%
"%msbuild%" "%rootdir%\src\Vts.SiteVisit\Vts.SiteVisit.csproj" %releaseWLbuildswitches%

"%msbuild%" "%rootdir%\src\Vts.Test\Vts.Test.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.Test\Vts.Test.csproj" %releasebuildswitches%

rem pause