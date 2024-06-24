$version = $args[0];
if (!$args) {
  $version="x.xx.x"
  echo "version set to x.xx.x"
  .\BuildTestCore.ps1
}

$vtslevel = $PWD

Write-Host "Create Scripting Release version = $version" -ForegroundColor Green
# Create release folder if it doesn't exist
$releasedir = ".\release"
if (-not (Test-Path -LiteralPath $releasedir)) {
    New-Item -Path $PWD -Name $releasedir -ItemType "directory"
}

# Create win-x64 zip
$archive="$PWD\release\VTS_Scripting_v" + $version + "_Win_x64.zip"
$source="$vtslevel\publish\win-x64\*"
Compress-Archive -Path $source -DestinationPath $archive -Force

# Create linux-x64 zip
$archive="$PWD\release\VTS_Scripting_v" + $version + "_Linux_x64.zip"
$source="$vtslevel\publish\linux-x64\*"
Compress-Archive -Path $source -DestinationPath $archive -Force

# Create osx-x64 zip
$archive="$PWD\release\VTS_Scripting_v" + $version + "_Mac_x64.zip"
$source="$vtslevel\publish\osx-x64\*"
Compress-Archive -Path $source -DestinationPath $archive -Force

Read-Host -Prompt "Press Enter to exit Scripting release process"
