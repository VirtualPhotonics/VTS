#This script is called from ~/vts/MonoBuild.sh so PWD=vts
SolutionDir=$PWD
NugetDir=$SolutionDir/src/.nuget
DestDir=$SolutionDir/src/packages

ProjectDir=$SolutionDir/src/Vts.Desktop
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.Desktop.Test
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.MonteCarlo.CommandLineApplication
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir
