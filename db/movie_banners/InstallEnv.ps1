$ErrorActionPreference = "Stop"

try 
{
  Push-Location $PSScriptRoot
  
  # Activate conda
  & $env:USERPROFILE\Anaconda3\shell\condabin\conda-hook.ps1 ; conda activate $env:USERPROFILE\Anaconda3
  
  $EnvName = "dotflik"
  $Environments = (& conda env list)
  $FoundEnvironment = ($Null -ne ($Environments | Where-Object { $_.Contains($EnvName) }))

  if ($FoundEnvironment)
  {
    Write-Warning "$EnvName environment is already installed"
  }
  else 
  {
    Write-Host "Installing $EnvName environment" -Foreground Cyan
    & conda env create -f dotflik.yaml
  }
}
finally
{
  # Conda has been activated, so now deactivate it twice. One is to deactivate 
  # the new installed environment, and second is to deactivate the "base" environment
  if ($Null -ne (Get-Command conda))
  {
    & conda deactivate
    & conda deactivate
  }
  Pop-Location
}

