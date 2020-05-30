$version = $args[0];
$runtime = $args[1];
Write-Host "version = $version runtime = $runtime" -ForegroundColor Green

$builddir = ".\build\$runtime"
New-Item -Path $PWD -Name $builddir -ItemType "directory"
$archive = $builddir + "\MC_v" + $version + "Beta.zip"
$source = "publish\$runtime\*"

Compress-Archive -Path $source -DestinationPath $archive 

$matlabfiles = "$PWD\matlab\post_processing\monte_carlo\simulation_result_loading\*"
Compress-Archive -Path $matlabfiles -Update -DestinationPath $archive

