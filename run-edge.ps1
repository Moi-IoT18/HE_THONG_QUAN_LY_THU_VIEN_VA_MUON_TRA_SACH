$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "QuanLyThuVien_UNETI01_DHTI17A4HN\QuanLyThuVien_UNETI01_DHTI17A4HN.csproj"
$serverUrl = "http://localhost:5020"
$pageUrl = "$serverUrl/Sach"

if (-not (Test-Path $projectPath)) {
    throw "Project not found: $projectPath"
}

if (Test-NetConnection -ComputerName "localhost" -Port 5020 -InformationLevel Quiet -WarningAction SilentlyContinue) {
    throw "Port 5020 is already in use. Stop the running app and try again."
}

$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
$env:ASPNETCORE_ENVIRONMENT = "Development"
try {
    $server = Start-Process -FilePath "dotnet" `
        -ArgumentList @("run", "--project", "`"$projectPath`"", "--no-launch-profile", "--urls", $serverUrl) `
        -NoNewWindow `
        -PassThru
}
finally {
    if ($null -eq $previousEnvironment) {
        Remove-Item Env:ASPNETCORE_ENVIRONMENT -ErrorAction SilentlyContinue
    }
    else {
        $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    }
}

$deadline = (Get-Date).AddSeconds(60)
$serverReady = $false
while ((Get-Date) -lt $deadline) {
    if ($server.HasExited) {
        throw "The app exited before the server became ready. Check the error output above."
    }

    if (Test-NetConnection -ComputerName "localhost" -Port 5020 -InformationLevel Quiet -WarningAction SilentlyContinue) {
        $serverReady = $true
        break
    }

    Start-Sleep -Seconds 1
}

if (-not $serverReady) {
    throw "The app did not open port 5020 within 60 seconds."
}

Start-Process -FilePath "microsoft-edge:$pageUrl"
$server.WaitForExit()
