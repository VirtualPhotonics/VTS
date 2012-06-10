@echo off
set currentdir=%~dp0

chdir %currentdir%
cd ..

set zip=%CD%\Tools\7zip\
set archive=%CD%\build\VTS_MATLAB_v1.0.4Beta.zip

set targetmatlabdir=%CD%\matlab\vts_wrapper\

"%zip%7za" a -r "%archive%" "%targetmatlabdir%*.*"

rem pause