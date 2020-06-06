$version = "4.10.0"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $version"
Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $version"

Read-Host -Prompt "Press Enter to exit"
