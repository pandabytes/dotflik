$ErrorActionPreference = "Stop"

try 
{
  Push-Location $PSScriptRoot
  
  # Activate conda
  & $env:USERPROFILE\Anaconda3\shell\condabin\conda-hook.ps1 ; conda activate $env:USERPROFILE\Anaconda3
  
  $EnvName = "dotflik"
  $Environments = & conda env list
  $FoundEnvironment = ($Null -ne ($Environments | Where-Object { $_.Contains($EnvName) }))

  if ($FoundEnvironment)
  {
    Write-Host "Uninstalling $EnvName environment" -Foreground Cyan
    & conda env remove -n $EnvName
    & conda clean -a -y
    & conda deactivate
    Remove-Item -Force -Recurse -Path $env:USERPROFILE\Anaconda3\envs\$EnvName
  }
}
finally
{
  # Conda has been activated, so now deactivate it
  if ($Null -eq (Get-Command conda))
  {
    & conda deactivate
  }
  Pop-Location
}

