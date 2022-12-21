$mccl_version = "7.1.0"
$matlab_version = "9.0.0"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $mccl_version"
Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $matlab_version"

Read-Host -Prompt "Press Enter to exit"
