Write-Host "Run vts_wrapper tests" -ForegroundColor Green
Write-Host "MATLAB not exiting nicely indicates test failure" -ForegroundColor Green
$vtslevel = $PWD

# Change current dir to vts\matlab\vts_wrapper and get rid of prior build results
cd "$vtslevel\matlab\vts_wrapper"
$matlablibdir = "$vtslevel\matlab\vts_wrapper\vts_libraries"
Remove-Item $matlablibdir -Recurse -ErrorAction Ignore
New-Item $matlablibdir -ItemType "directory"

# put supporting libraries into vts_libraries folder
$vtsdesktop = "$vtslevel\publish\win-x64"
Copy-Item -Path "$vtsdesktop\*" -Destination "$matlablibdir"

# UNCOMMENT NEXT LINE WHEN FILES IN vts_libraries
matlab -wait -r "vts_tests; quit" 
#matlab -r "mc_tests; structures_tests; quit" # to only run unit tests

# return to vts level
cd $vtslevel
