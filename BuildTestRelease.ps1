$mccl_version = "5.1.0"
$matlab_version = "4.11.0"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $mccl_version"
Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $matlab_version"

Read-Host -Prompt "Press Enter to exit"
