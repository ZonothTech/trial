<#
.SYNOPSIS
    Creates the TrailShop training Git history (Windows PowerShell 5.1 or PowerShell 7+).

.DESCRIPTION
    The history deliberately contains a SYNTHETIC SMTP password that is added in one
    commit and removed in the next. Session 7 uses it to practise finding secrets in
    history. The value is fake and has never worked anywhere.

    It also creates a local bare repository next to this folder and registers it as
    "origin" (Claude Code's /security-review compares your branch with origin's
    default branch, so it needs one). Use -NoOrigin to skip that step, for example
    when you will push to GitHub and use that as origin instead.

    Run once:  powershell -ExecutionPolicy Bypass -File .\tools\Initialize-TrainingRepo.ps1 [-NoOrigin]
    Result:    a repo on branch "main" with three commits and the tag "course-start".
#>
[CmdletBinding()]
param(
    [switch]$NoOrigin
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

if (Test-Path (Join-Path $repoRoot '.git')) {
    throw "A .git folder already exists in $repoRoot. Refusing to touch existing history."
}

# Refuse to run inside another repository (for example a clone of the course's student
# repository): the history and the local origin must live in TrailShop's own folder.
# (Error preference relaxed for this probe: Windows PowerShell 5.1 turns git's stderr into a terminating error under 'Stop'.)
$ErrorActionPreference = 'Continue'
$outer = & git -C $repoRoot rev-parse --show-toplevel 2>$null
$outerExit = $LASTEXITCODE
$ErrorActionPreference = 'Stop'
if ($outerExit -eq 0 -and $outer) {
    throw "This folder is inside another Git repository ($outer). Copy the TrailShop folder somewhere outside it (for example C:\src\TrailShop) and run this script there."
}
$global:LASTEXITCODE = 0

$devSettings = Join-Path $repoRoot 'src/TrailShop.Api/appsettings.Development.json'
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Invoke-Git {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$GitArgs)
    & git @GitArgs
    if ($LASTEXITCODE -ne 0) { throw "git $($GitArgs -join ' ') failed with exit code $LASTEXITCODE" }
}

function New-TrainingCommit {
    param([string]$Date, [string]$Message)
    $env:GIT_AUTHOR_DATE = $Date
    $env:GIT_COMMITTER_DATE = $Date
    try {
        Invoke-Git -c 'user.name=TrailShop Trainer' -c 'user.email=trainer@trailshop.example' -c 'commit.gpgsign=false' commit --quiet -m $Message
    }
    finally {
        Remove-Item Env:GIT_AUTHOR_DATE, Env:GIT_COMMITTER_DATE -ErrorAction SilentlyContinue
    }
}

$withSmtp = @'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Smtp": {
    "Host": "smtp.trailshop.invalid",
    "Port": 587,
    "Username": "orders@trailshop.example",
    "Password": "Wint3r-Trail-2026!"
  }
}
'@

$withoutSmtp = @'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
'@

Invoke-Git init --quiet --initial-branch=main
Invoke-Git add --all
New-TrainingCommit -Date '2026-09-01T10:00:00Z' -Message 'Initial TrailShop import'

# Commit 2: a developer adds SMTP settings, including a (synthetic) password.
[System.IO.File]::WriteAllText($devSettings, ($withSmtp -replace "`r`n", "`n") + "`n", $utf8NoBom)
Invoke-Git add -- 'src/TrailShop.Api/appsettings.Development.json'
New-TrainingCommit -Date '2026-09-08T16:20:00Z' -Message 'Configure SMTP for order confirmation emails'

# Commit 3: the settings are removed again. The password is still in history.
[System.IO.File]::WriteAllText($devSettings, ($withoutSmtp -replace "`r`n", "`n") + "`n", $utf8NoBom)
Invoke-Git add -- 'src/TrailShop.Api/appsettings.Development.json'
New-TrainingCommit -Date '2026-09-09T09:05:00Z' -Message 'Remove SMTP settings from appsettings'

Invoke-Git tag course-start

if (-not $NoOrigin) {
    $originDir = Join-Path (Split-Path -Parent $repoRoot) ((Split-Path -Leaf $repoRoot) + '-origin.git')
    if (Test-Path $originDir) {
        Write-Warning "Skipping local origin: $originDir already exists."
    }
    else {
        Invoke-Git init --quiet --bare --initial-branch=main $originDir
        Invoke-Git remote add origin $originDir
        Invoke-Git push --quiet --set-upstream origin main
        Invoke-Git push --quiet origin course-start
        Invoke-Git remote set-head origin main
        Write-Host "Local origin created at $originDir"
    }
}

Write-Host 'TrailShop training history created:'
Invoke-Git log --oneline --decorate
