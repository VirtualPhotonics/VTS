$version = $args[0];
Write-Host "version = $version" -ForegroundColor Green

$archive="$PWD\build\VTS_MATLAB_v" + $version + "Beta.zip"
$source="$PWD\matlab\vts_wrapper\*"

Compress-Archive -Path $source -DestinationPath $archive