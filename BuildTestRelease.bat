rem ********** RUN THIS FILE AS ADMINISTRATOR **********
set rootdir=%~dp0
set version=2.0.0
set EnableNuGetPackageRestore=true

rem ********** CHANGE CURRENT DIR TO LOCATION OF BAT FILE **********
cd %~dp0

rem ********** CLEAN THE MC RELEASE FOLDERS **********
rmdir "%rootdir%build" /s /q
rmdir "%rootdir%src\Vts.MonteCarlo.CommandLineApplication\bin" /s /q
rmdir "%rootdir%matlab\vts_wrapper\vts_libraries" /s /q
rmdir "%rootdir%matlab\vts_wrapper\results" /s /q
rmdir "%rootdir%matlab\vts_wrapper\results_test1" /s /q
rmdir "%rootdir%matlab\vts_wrapper\results_test2" /s /q
# tried to use wild card above with no luck


rem ********** BUILD THE DESKTOP VERSION **********
call "%rootdir%DesktopBuild.bat"
call "%rootdir%DesktopTests.bat"


rem ********** CREATE THE RELEASE PACKAGES **********
call "%rootdir%src\Vts.MonteCarlo.CommandLineApplication\CreateRelease.bat" %version%
call "%rootdir%matlab\CreateRelease.bat" %version% 


rem ********** RUN MATLAB INTEROP AND MONTE CARLO POST-PROCESSING TESTS **********
# call "%rootdir%RunMatlabUnitTests.bat"

rem ********** BUILD THE SILVERLIGHT VERSION **********
 call "%rootdir%SilverlightBuild.bat"

cd "%rootdir%src\Vts.Test\bin\Debug\"
start TestPage.html

pause

cd "%rootdir%src\Vts.Gui.Silverlight\bin\ReleaseWhiteList\"
start TestPage.html