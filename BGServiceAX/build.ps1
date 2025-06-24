# build.ps1

$projectFile = "AXAPIWrapper.csproj"
$configuration = "Release"
$publishDir = "D:\cw_publish\AXSDK-API"
$sevenZipExe = "C:\Program Files\7-Zip\7z.exe"

Write-Host "📦 Reading project info from $projectFile..."

[xml]$csprojXml = Get-Content $projectFile

$version = $csprojXml.Project.PropertyGroup.Version.Trim()
$framework = $csprojXml.Project.PropertyGroup.TargetFramework.Trim()

# Safe fallback for missing <AssemblyName>
$appNameNode = $csprojXml.Project.PropertyGroup.AssemblyName
$appName = if ($appNameNode) { $appNameNode.Trim() } else { [System.IO.Path]::GetFileNameWithoutExtension($projectFile) }

if (-not $version) {
    Write-Error "❌ <Version> not found in $projectFile"
    exit 1
}
if (-not $framework) {
    Write-Error "❌ <TargetFramework> not found in $projectFile"
    exit 1
}

# Get time and git hash
$timestamp = Get-Date -Format "yyyyMMddHHmm"
try {
    $gitHash = (git rev-parse --short HEAD) -replace "`n",""
    if (-not $gitHash) { $gitHash = "nogit" }
} catch {
    $gitHash = "nogit"
}
Write-Host "📅 AppName: $appName"
Write-Host "📌 Version: $version"
Write-Host "📌 Framework: $framework"
Write-Host "📌 Timestamp: $timestamp"
Write-Host "📌 Git Commit: $gitHash"
Write-Host "📁 Publish Output: $publishDir"

# Ensure publish directory exists
if (-not (Test-Path $publishDir)) {
    Write-Host "📂 Creating publish directory..."
    New-Item -Path $publishDir -ItemType Directory | Out-Null
}

# Clean, Build, Publish
Write-Host "`n🧹 Cleaning..."
dotnet clean $projectFile

Write-Host "`n🔨 Building..."
dotnet build $projectFile -c $configuration

Write-Host "`n🚀 Publishing..."
dotnet publish $projectFile -c $configuration -o $publishDir

# Rename appsettings.json → appsettings.publish.json
$appsettingsPath = Join-Path $publishDir "appsettings.json"
$appsettingsRenamed = Join-Path $publishDir "appsettings.publish.json"

if (Test-Path $appsettingsPath) {
    Write-Host "🔐 Renaming appsettings.json → appsettings.publish.json"
    Move-Item -Path $appsettingsPath -Destination $appsettingsRenamed -Force
} else {
    Write-Warning "⚠️ appsettings.json not found. Skipping rename."
}

# ZIP using 7-Zip
$zipFileName = "$appName-v$version-update-$timestamp-$gitHash.zip"
$zipPath = Join-Path $publishDir $zipFileName

Write-Host "`n📦 Creating ZIP with 7-Zip: $zipPath"

if (-not (Test-Path $sevenZipExe)) {
    Write-Warning "⚠️  7-Zip not found at '$sevenZipExe'. Skipping ZIP step."
} else {
    $arguments = @(
        'a',
        '-tzip',
        "`"$zipPath`"",
        "`"$publishDir\*`"",
        '-xr!*.zip',
        '-xr!*.rar',
        "-xr!$zipFileName"
    )

    Write-Host "▶️ Running 7-Zip..."
    Start-Process -FilePath $sevenZipExe -ArgumentList $arguments -Wait -NoNewWindow

    if (Test-Path $zipPath) {
        Write-Host "✅ ZIP file created: $zipPath"
    } else {
        Write-Warning "❌ ZIP file was not created."
    }
}

