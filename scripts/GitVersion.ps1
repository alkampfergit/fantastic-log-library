<#
.SYNOPSIS
Run gitversion and if script is running in azure devops build change
build number and optionally changes assemblyinfo.cs and vb.

#>
param (
    [Parameter(Mandatory=$true)]
    [string]$outFolder,

    [string]$pipelineName,

    [Parameter(Mandatory=$true)]
    [string]$sourceFolder,

    [Boolean] $dumpVariables = $false,
    
    [Boolean] $modifyAssemblyInfoFiles = $false
)

# This simple powershell script was created to run dotnet-gitversion
# in a uniform way for every project that was not compiled with 
# dotnet and build script like Connector.
Write-Host "restoring tooling for gitversion"
dotnet tool restore

# dotnet tool run dotnet-gitversion /config GitVersion.yml

Write-Host "Running gitversion to determine version"
$version = dotnet tool run dotnet-gitversion /config GitVersion.yml | Out-String | ConvertFrom-Json
Write-Output $version

$assemblyVer = $version.AssemblySemVer
$assemblyFileVer = $version.AssemblySemFileVer
$nugetVersion = $version.NuGetVersionV2
$assemblyInformationalVersion = $version.FullSemVer + "." + $version.Sha
$fullSemver = $version.FullSemVer

Write-Host "Assembly version is $assemblyVer"
Write-Host "File version is $assemblyFileVer"
Write-Host "Nuget version is $nugetVersion"
Write-Host "Informational version is $assemblyInformationalVersion"
 
$buildId = $env:BUILD_BUILDID
Write-Host "Build id variable is $buildId"
if (![System.String]::IsNullOrEmpty($buildId)) 
{
    Write-Host "Running in an Azure Devops Build"

    Write-Host "##vso[build.updatebuildnumber]$pipelineName - $fullSemver"
    Write-Host "##vso[task.setvariable variable=assemblyVer;]$assemblyVer"
    Write-Host "##vso[task.setvariable variable=assemblyFileVer;]$assemblyFileVer"
    Write-Host "##vso[task.setvariable variable=nugetVersion;]$nugetVersion"
    Write-Host "##vso[task.setvariable variable=assemblyInformationalVersion;]$assemblyInformationalVersion"

    if ($dumpVariables) 
    {
        Write-Host "Dumping all environment variable of the build"
        $var = (gci env:*).GetEnumerator() | Sort-Object Name
        $out = ""
        Foreach ($v in $var) {$out = $out + "`t{0,-28} = {1,-28}`n" -f $v.Name, $v.Value}

        write-output "dump variables on $outFolder\EnvVar.md"
        $fileName = "$outFolder\EnvVar.md"
        set-content $fileName $out
        write-output "##vso[task.addattachment type=Distributedtask.Core.Summary;name=Environment Variables;]$fileName"
    }

    if ($modifyAssemblyInfoFiles) 
    {
      Write-Output "Modifying all assemblyinfo files to add versioning on $sourceFolder"
      Update-SourceVersion $sourceFolder $assemblyVer $assemblyFileVer $assemblyInformationalVersion
    }
}

