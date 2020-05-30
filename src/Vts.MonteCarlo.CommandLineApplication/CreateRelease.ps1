$version = $args[0];
Write-Host "version = $version" -ForegroundColor Green

$7zip = ".\tools\7zip\7za.exe"
$archive = ".\build\MC_v" + $version + "Beta.zip "

$targetdir=".\publish\win-x64\"

# publish folder had
#Compress-Archive -Path .\publish\win-x64\mc.exe -Update -DestinationPath $archive
#Compress-Archive -Path .\publish\win-x64\*.dll -Upate -DestinationPath $archive
.\tools\7zip\7za.exe a $archive .\publish\win-x64\mc.exe
.\tools\7zip\7za.exe a $archive .\publish\win-x64\*.dll
.\tools\7zip\7za.exe a $archive .\publish\win-x64\*.txt
.\tools\7zip\7za.exe a $archive .\publish\win-x64\*.config
.\tools\7zip\7za.exe a $archive .\publish\win-x64\mc_post.exe
.\tools\7zip\7za.exe a $archive .\publish\win-x64\mc_post.exe.config
#$7zip $archive $targetdir + "mc.exe"
#$7zip $archive $targetdir + "*.dll"
#$7zip $archive $targetdir + "*.txt"
#$7zip $archive $targetdir + "*.config"

#$targetmatlabdir=".\matlab\post_processing\monte_carlo\simulation_result_loading\"
.\tools\7zip\7za.exe a $archive .\matlab\post_processing\monte_carlo\simulation_result_loading\*

