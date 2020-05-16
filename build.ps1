<#
.SYNOPSIS
Execute a complete build for the project

.DESCRIPTION
Build / test / release standard.

.EXAMPLE

.NOTES

.PARAMETER configuration
debug or release, it standard configuration parameter of dotnet --configuration

.PARAMETER outFolder 
Address of the folder where the script will output all the output.

.PARAMETER skipTests
If $true the script will skip test phase (save time when you test the script itself :) )

.PARAMETER pipelineName
Used only if we run in continous environement. Actually supported CI environment is only AzDo
#>
param(
  $configuration = 'release',
  $outFolder = "build_output",
  $skipTests = $false,
  $pipelineName = ""
)
$runningDirectory = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

Write-Host "Running directory is $runningDirectory"
#Unload and reload build utils module 
if(-not(Get-Module -name BuildUtils)) 
{
  Write-Host "Loading Build utils"
  Import-Module -Name "$runningDirectory\scripts\BuildUtils"
}

$pathRooted = [System.IO.Path]::IsPathRooted($outFolder)
if ($pathRooted -eq $false) 
{
    $outFolder = [System.IO.Path]::Combine($runningDirectory, $outFolder)
}

Write-Host "Output directory is $outFolder"
if ((Test-Path $outFolder) -eq $true) 
{
    Remove-Item $outFolder -Recurse -Force -ErrorAction SilentlyContinue
}

New-Item -ItemType Directory $outFolder

Write-Host "Checking .net core version"

dotnet --version

dotnet restore src/FantasticLogLibrary.sln

. .\scripts\GitVersion.ps1 -outFolder $outFolder -sourceFolder $runningDirectory -pipelineName $pipelineName

if ($skipTests -eq $false) 
{
    Write-Host "running tests looking in $runningDirectory"

    $testProject = Get-ChildItem $runningDirectory -Recurse -Filter "*.tests.csproj"
    foreach ($file in $testProject) 
    {
        Write-Host "Run test for $($file.FullName)"
        dotnet test $file.FullName --configuration debug --logger "trx;LogFileName=$outFolder/TEST-$file.trx" /p:PackageVersion=$nugetVersion /p:AssemblyVersion=$assemblyVer /p:FileVersion=$assemblyFileVer /p:InformationalVersion=$assemblyInformationalVersion
    }
}
Write-Host "Now we compile in release to generate correct assemblies"
Write-Host "Versions are: NugetVersion=$nugetVersion\n assemblyVer=$assemblyVer\n assemblyFileVer=$assemblyFileVer\n assemblyInformationalVersion=$assemblyInformationalVersion"
dotnet build src/FantasticLogLibrary.sln --configuration $configuration /p:PackageVersion=$nugetVersion /p:AssemblyVersion=$assemblyVer /p:FileVersion=$assemblyFileVer /p:InformationalVersion=$assemblyInformationalVersion
