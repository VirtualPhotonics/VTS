$mccl_version = "7.3.0"
$matlab_version = "10.0.0"

Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $mccl_version"
# only run if matlab installed
if (Get-Command "matlab" -ErrorAction SilentlyContinue) 
{
  Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $matlab_version"
}

Read-Host -Prompt "Press Enter to exit"
