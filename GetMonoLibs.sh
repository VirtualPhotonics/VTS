#This script is called from ~/vts/MonoBuild.sh so PWD=vts

# tell mono certs to trust when doing https:
mozroots --import --sync
# use an environment variable to enable package restore 
export EnableNuGetPackageRestore=true

# set up to call nuget from script "nuget" in this folder
SolutionDir=$PWD
NugetDir=$SolutionDir/src/.nuget
DestDir=$SolutionDir/src/packages

ProjectDir=$SolutionDir/src/Vts.Desktop
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.Desktop.Test
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.MonteCarlo.CommandLineApplication
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.MonteCarlo.CommandLineApplication.Test
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.MonteCarlo.PostProcessor
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir

ProjectDir=$SolutionDir/src/Vts.MonteCarlo.PostProcessor.Test
$NugetDir/nuget install $ProjectDir/packages.config -o $DestDir
