$mccl_version = "5.0.0"
$matlab_version = "4.10.0"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $mccl_version"
Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $matlab_version"

Read-Host -Prompt "Press Enter to exit"
