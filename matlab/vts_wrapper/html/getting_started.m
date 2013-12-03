%% Getting Started with VTS in MATLAB
% These are instructions on how to get started using the VTS libraries in
% MATLAB.
%
%% Prerequisites
% MATLAB for Windows R2011b or higher
%
% <html>
% .NET Framework 4.0 - download <a href="http://www.microsoft.com/downloads/en/details.aspx?familyid=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en">here</a>
% </html>
% 
% *Note: Calling the VTS libraries from MATLAB will only work on the Windows version of MATLAB.* 
%% Setting up the Current Folder
% Make sure that the current folder in MATLAB is set to the root of the
% unzipped VTS MATLAB Package download or vts_wrapper if you have the full 
% source code. 
%
% *Note: If you navigate down into the directory structure, these VTS
% help files will no longer be accessible.*
%
%% Run the Demos
% The *startup.m* script adds the main sub folders of the MATLAB package to
% the path, the folders html, vts and vts_tests should no longer be greyed
% out. This script will also load the VTS assemblies needed for running the
% VTS (this folder can remain greyed out).
%
% * Bring up the demo file *vts_solver_demo.m* or *vts_mc_demo.m*
% * To run all the demos, hit F5 or go to Run in the Debug menu
% * To run one example, highlight the cell by clicking into it and press CTRL + Enter
%
%% Create your own example
% Create a new script file in the root of the VTS package folder and add
% the following code to the top of the file:
%   
%   clear all
%   clc
%   startup();
%
% Select the example that is the closest to what you are trying to achieve
% and modify the parameters accordingly. Run the file.
%%