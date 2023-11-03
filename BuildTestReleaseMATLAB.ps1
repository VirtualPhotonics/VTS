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
# RunMATLABUnitTests copies Vts.Desktop/bin/Release files to matlab/vts_wrapper/vts_libraries
.\RunMATLABUnitTests.ps1 

Write-Host "Create MATLAB Release version = $version" -ForegroundColor Green
# Create release folder if it doesn't exist
$releasedir = ".\release"
if (-not (Test-Path -LiteralPath $releasedir)) {
    New-Item -Path $PWD -Name $releasedir -ItemType "directory"
}
$archive="$PWD\release\VTS_MATLAB_v" + $version + ".zip"
$source="$matlabdir\vts_wrapper\*"

Compress-Archive -Path $source -DestinationPath $archive -Force

Read-Host -Prompt "Press Enter to exit MATLAB release process"
