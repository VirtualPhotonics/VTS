rem @echo off
set rootdir=%~dp0
set EnableNuGetPackageRestore=true

set nunit=%rootdir%\tools\nunit\Desktop\

"%nunit%"nunit3-console /result:TestResult-Vts-Debug.xml "%rootdir%\src\Vts.Desktop.Test\bin\Debug\Vts.Desktop.Test.dll" 
"%nunit%"nunit3-console /result:TestResult-MCCL-Debug.xml "%rootdir%\src\Vts.MonteCarlo.CommandLineApplication.Test\bin\Debug\Vts.MonteCarlo.CommandLineApplication.Test.dll" 
"%nunit%"nunit3-console /result:TestResult-MCPP-Debug.xml "%rootdir%\src\Vts.MonteCarlo.PostProcessor.Test\bin\Debug\Vts.MonteCarlo.PostProcessor.Test.dll"

"%nunit%"nunit3-console /result:TestResult-Vts-Release.xml "%rootdir%\src\Vts.Desktop.Test\bin\Release\Vts.Desktop.Test.dll"
"%nunit%"nunit3-console /result:TestResult-MCCL-Release.xml "%rootdir%\src\Vts.MonteCarlo.CommandLineApplication.Test\bin\Release\Vts.MonteCarlo.CommandLineApplication.Test.dll"
"%nunit%"nunit3-console /result:TestResult-MCPP-Release.xml "%rootdir%\src\Vts.MonteCarlo.PostProcessor.Test\bin\Release\Vts.MonteCarlo.PostProcessor.Test.dll"

rem pause