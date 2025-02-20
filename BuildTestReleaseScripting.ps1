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

# Create library zip
$archive="$PWD\release\VTS_Scripting_v" + $version + ".zip"
$source="$vtslevel\publish\local\*"
Compress-Archive -Path $source -DestinationPath $archive -Force

Read-Host -Prompt "Scripting release complete, press Enter to exit Scripting release process"
