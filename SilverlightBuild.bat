set rootdir=%CD%

PATH=%PATH%;%WINDIR%\Microsoft.Net\Framework\v4.0.30319

msbuild "%rootdir%"\src\Vts\Vts.csproj
msbuild "%rootdir%"\src\Vts.SiteVisit\Vts.SiteVisit.csproj

pause