@echo off
set currentdir=%~dp0

chdir %currentdir%
cd ..\..

set version=%1

if "%version%" == "" (set /p version=Enter the version number: )

set zip=%CD%\Tools\7zip\
set archive=%CD%\build\MC_v%version%Beta.zip

set targetdir=%CD%\build\apps\mc\Release\
set targetmatlabdir=%CD%\matlab\post_processing\monte_carlo\simulation_result_loading\


"%zip%"7za a "%archive%" "%targetdir%"mc.exe

"%zip%"7za a "%archive%" "%targetdir%"*.dll

"%zip%"7za a "%archive%" "%targetdir%"*.txt

"%zip%"7za a "%archive%" "%targetdir%"*.config

"%zip%"7za a -r "%archive%" "%targetmatlabdir%"*.*

rem pause