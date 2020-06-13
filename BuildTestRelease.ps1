$version = "4.10.0"
$release = "win-x64"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $version $release"
Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $version"

Read-Host -Prompt "Press Enter to exit"
