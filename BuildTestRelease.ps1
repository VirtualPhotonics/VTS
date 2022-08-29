$mccl_version = "7.0.0"
$matlab_version = "8.0.0"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $mccl_version"
Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $matlab_version"

Read-Host -Prompt "Press Enter to exit"
