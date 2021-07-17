$version = $args[0];
if (!$args) {
  $version="x.xx.x"
  echo "version set to x.xx.x"
  .\BuildTestCore.ps1
}

Write-Host "Release Packages" -ForegroundColor Green
Write-Host "Clean Release folders" -ForegroundColor Green
Remove-Item "$PWD/matlab/vts_wrapper/vts_libraries" -Recurse -ErrorAction Ignore
Remove-Item "$PWD/matlab/vts_wrapper/results*" -Recurse -ErrorAction Ignore

Write-Host "Test MATLAB unit tests" -ForegroundColor Green
# RunMATLABUnitTests copies Vts.Desktop/bin/Release files to matlab/vts_wrapper/vts_libraries
.\RunMATLABUnitTests.ps1 

Write-Host "Create MATLAB Release version = $version" -ForegroundColor Green
$archive="$PWD\release\VTS_MATLAB_v" + $version + "Beta.zip"
$source="$PWD\matlab\vts_wrapper\*"

Compress-Archive -Path $source -DestinationPath $archive

Read-Host -Prompt "Press Enter to exit MATLAB release process"
