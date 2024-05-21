$mccl_version = "7.3.0"
$vts_version = "10.0.0"

Write-Host "Create clean publish and release folders" -ForegroundColor Green
Remove-Item "$PWD\release" -Recurse -ErrorAction Ignore
Remove-Item $PWD\publish -Recurse -ErrorAction Ignore

.\BuildTestCore.ps1 

Invoke-Expression ".\BuildTestReleaseScripting.ps1 $vts_version"
Invoke-Expression ".\BuildTestReleaseMCCL.ps1 $mccl_version"
# only run if matlab installed
if (Get-Command "matlab" -ErrorAction SilentlyContinue) 
{
  Invoke-Expression ".\BuildTestReleaseMATLAB.ps1 $vts_version"
}

Read-Host -Prompt "Press Enter to exit"
