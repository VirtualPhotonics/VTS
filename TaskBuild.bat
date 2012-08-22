set rootdir=%~dp0
set debugbuildswitches=/p:WarningLevel=2 /nologo /v:n
set releasebuildswitches=/p:Configuration=Release /p:WarningLevel=2 /nologo /v:n
set msbuild=%WINDIR%\Microsoft.Net\Framework64\v4.0.30319\msbuild

"%msbuild%" "%rootdir%\src\Vts.ImportSpectralData.Desktop\Vts.ImportSpectralData.Desktop.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.ImportSpectralData.Desktop\Vts.ImportSpectralData.Desktop.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.GenerateReferenceData\Vts.MonteCarlo.GenerateReferenceData.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.MonteCarlo.GenerateReferenceData\Vts.MonteCarlo.GenerateReferenceData.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.ReportForwardSolvers.Desktop\Vts.ReportForwardSolvers.Desktop.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.ReportForwardSolvers.Desktop\Vts.ReportForwardSolvers.Desktop.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.ReportInverseSolver.Desktop\Vts.ReportInverseSolver.Desktop.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.ReportInverseSolver.Desktop\Vts.ReportInverseSolver.Desktop.csproj" %releasebuildswitches%

"%msbuild%" "%rootdir%\src\Vts.WriteNurbsValues.Desktop\Vts.WriteNurbsValues.Desktop.csproj" %debugbuildswitches%
"%msbuild%" "%rootdir%\src\Vts.WriteNurbsValues.Desktop\Vts.WriteNurbsValues.Desktop.csproj" %releasebuildswitches%

rem pause
