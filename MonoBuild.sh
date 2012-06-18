SolutionDir=$PWD

#Build the solution in Debug configuration
mdtool build --configuration:Debug $SolutionDir/src/Vts-Mono.sln

#Build the solution in Release configuration
mdtool build --configuration:Release $SolutionDir/src/Vts-Mono.sln

#Post build events:
ConfigName=Debug
Source=$SolutionDir/src/Vts.Desktop/bin
Destination=$SolutionDir/build/core/Desktop

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Release

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Debug
Source=$SolutionDir/src/Vts.MonteCarlo.PostProcessor/bin
Destination=$SolutionDir/build/apps/mc_post

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Release

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Debug
Source=$SolutionDir/src/Vts.MonteCarlo.CommandLineApplication/bin
Destination=$SolutionDir/build/apps/mc

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Release

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

#Generate the XML input files
cd $Destination/$ConfigName
mono mc.exe geninfiles
