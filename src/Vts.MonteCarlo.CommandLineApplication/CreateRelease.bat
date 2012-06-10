@echo off
set currentdir=%~dp0

chdir %currentdir%
cd ..\..

set zip=%CD%\Tools\7zip\
set archive=%CD%\build\MC_v1.0.4Beta.zip

set targetdir=%CD%\build\apps\mc\Release\
set targetmatlabdir=%CD%\matlab\post_processing\monte_carlo\simulation_result_loading\


"%zip%"7za a "%archive%" "%targetdir%"mc.exe

"%zip%"7za a "%archive%" "%targetdir%"*.dll

"%zip%"7za a "%archive%" "%targetdir%"*.txt

"%zip%"7za a "%archive%" "%targetdir%"*.config

"%zip%"7za a "%archive%" "%targetdir%"infile_*.xml

"%zip%"7za a -r "%archive%" "%targetmatlabdir%"*.*

rem pause