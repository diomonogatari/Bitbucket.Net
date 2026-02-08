[CmdletBinding()]
param(
    [Parameter()]
    [switch]$Fix
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$rootDir = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$solutionPath = Join-Path $rootDir 'Bitbucket.Net.slnx'

if (-not (Test-Path -LiteralPath $solutionPath)) {
    throw "Solution not found at: $solutionPath"
}

$verifyArgs = @()
if (-not $Fix) {
    $verifyArgs += '--verify-no-changes'
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'dotnet is required but was not found in PATH.'
}

if ($env:SKIP_DOTNET_FORMAT_WHITESPACE -ne '1') {
    Write-Verbose "Running: dotnet format whitespace $solutionPath"
    dotnet format whitespace $solutionPath @verifyArgs
}
else {
    Write-Verbose 'Skipping whitespace formatting (SKIP_DOTNET_FORMAT_WHITESPACE=1)'
}

Write-Verbose "Running: dotnet format style $solutionPath"
dotnet format style $solutionPath --severity warn @verifyArgs
