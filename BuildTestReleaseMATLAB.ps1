$version = $args[0];
if (!$args) {
  $version="x.xx.x"
  echo "version set to x.xx.x"
  .\BuildTestCore.ps1
}
$matlabdir = "$PWD\matlab"
Write-Host "Release Packages" -ForegroundColor Green
Write-Host "Clean Release folders" -ForegroundColor Green
Remove-Item "$matlabdir/vts_wrapper/vts_libraries" -Recurse -ErrorAction Ignore
Remove-Item "$matlabdir/vts_wrapper/results*" -Recurse -ErrorAction Ignore

Write-Host "Test MATLAB unit tests" -ForegroundColor Green
# RunMATLABUnitTests copies Vts\publish\local files to matlab/vts_wrapper/vts_libraries
Write-Host "Run vts_wrapper tests" -ForegroundColor Green
Write-Host "MATLAB not exiting nicely indicates test failure" -ForegroundColor Green
$vtslevel = $PWD

# Change current dir to vts\matlab\vts_wrapper and get rid of prior build results
cd "$vtslevel\matlab\vts_wrapper"
$matlablibdir = "$vtslevel\matlab\vts_wrapper\vts_libraries"
Remove-Item $matlablibdir -Recurse -ErrorAction Ignore
New-Item $matlablibdir -ItemType "directory"

# put supporting libraries into vts_libraries folder
$vtslibraries = "$vtslevel\publish\local"
Copy-Item -Path "$vtslibraries\*" -Destination "$matlablibdir"

matlab -wait -r "vts_tests; quit"

# return to vts level
cd $vtslevel


Write-Host "Create MATLAB Release version = $version" -ForegroundColor Green
# Create release folder if it doesn't exist
$releasedir = ".\release"
if (-not (Test-Path -LiteralPath $releasedir)) {
    New-Item -Path $PWD -Name $releasedir -ItemType "directory"
}
$archive="$PWD\release\VTS_MATLAB_v" + $version + ".zip"
$source="$matlabdir\vts_wrapper\*"

Compress-Archive -Path $source -DestinationPath $archive -Force

Read-Host -Prompt "MATLAB release complete, press Enter to exit MATLAB release process"
