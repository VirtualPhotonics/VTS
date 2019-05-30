#This script is called from ~/vts/MonoBuild.sh so PWD=vts

# tell mono certs to trust when doing https:
# following is version for ubuntu or debian dists else RedHat dist
if [ -f /etc/lsb-release ]; then
  sudo cert-sync /etc/ssl/certs/ca-certificates.crt
elif [ -f /etc/redhat-release ]; then
  sudo cert-sync /etc/pki/tls/certs/ca-bundle.crt
fi
# prior method that is now deprecated
# mozroots --import --sync 

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
