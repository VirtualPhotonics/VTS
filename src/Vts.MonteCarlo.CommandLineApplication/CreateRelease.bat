@echo off

set currentdir=%CD%

chdir ..\..

set archive=%currentdir%\MC_v1.0.4Beta.zip

set targetdir=%CD%\build\apps\mc\Release\
set targetmatlabdir=%CD%\matlab\post_processing\monte_carlo\simulation_result_loading\


"%currentdir%"\7za a "%archive%" "%targetdir%"mc.exe

"%currentdir%"\7za a "%archive%" "%targetdir%"*.dll

"%currentdir%"\7za a "%archive%" "%targetdir%"*.txt

"%currentdir%"\7za a "%archive%" "%targetdir%"*.config

"%currentdir%"\7za a "%archive%" "%targetdir%"infile_*.xml

"%currentdir%"\7za a -r "%archive%" "%targetmatlabdir%"*.*

rem pause