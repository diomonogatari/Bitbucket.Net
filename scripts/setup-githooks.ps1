[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw 'git is required but was not found in PATH.'
}

$rootDir = (git rev-parse --show-toplevel 2>$null)
if (-not $rootDir) {
    throw 'Not inside a git repository.'
}

Push-Location -LiteralPath $rootDir
try {
    git config core.hooksPath .githooks
    Write-Output 'Git hooks enabled (core.hooksPath=.githooks).'
}
finally {
    Pop-Location
}
