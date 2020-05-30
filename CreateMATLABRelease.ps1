$version = $args[0];
$runtime = $args[1];

$builddir = ".\build\$runtime"
$archive = $builddir + "\VTS_MATLAB_v" + $version + "Beta.zip"

$matlabfiles = "$PWD\matlab\vts_wrapper\*"
Compress-Archive -Path $matlabfiles -DestinationPath $archive
