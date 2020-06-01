rem @echo off
set currentdir=%~dp0

set version=%1

if "%version%" == "" (set /p version=Enter the version number: )

set zip=%CD%\Tools\7zip\
set archive=%CD%\build\VTS_MATLAB_v%version%Beta.zip

set targetmatlabdir=%CD%\matlab\vts_wrapper\

"%zip%7za" a -r "%archive%" "%targetmatlabdir%*.*"

rem pause