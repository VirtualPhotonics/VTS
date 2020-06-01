rem @echo off
set rootdir=%~dp0
set EnableNuGetPackageRestore=true

set nunit=%rootdir%\tools\nunit\Desktop\

"%nunit%"nunit3-console /result:TestResult-Vts-Debug.xml "%rootdir%\src\Vts.Desktop.Test\bin\Debug\Vts.Desktop.Test.dll" 
"%nunit%"nunit3-console /result:TestResult-Vts-Release.xml "%rootdir%\src\Vts.Desktop.Test\bin\Release\Vts.Desktop.Test.dll"

rem pause