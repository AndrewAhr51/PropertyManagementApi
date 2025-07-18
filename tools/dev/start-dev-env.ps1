Write-Host "🚀 Starting OmniTenant Dev Environment..."

# Start ngrok in background
Start-Process ngrok -ArgumentList "http https://localhost:7144" -WindowStyle Hidden
Start-Sleep -Seconds 3

# Get HTTPS tunnel
$tunnelUrl = $null
for ($i = 0; $i -lt 5; $i++) {
    try {
        $tunnels = (Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels").tunnels
        $tunnelUrl = $tunnels | Where-Object { $_.public_url -like "https*" } | Select-Object -First 1 | ForEach-Object { $_.public_url }
        if ($tunnelUrl) { break }
    } catch { Start-Sleep -Seconds 2 }
}
if (-not $tunnelUrl) { throw "❌ No HTTPS tunnel detected." }
Write-Host "🔗 ngrok tunnel: $tunnelUrl"

# Patch appsettings.Development.json
$configPath = Join-Path $PSScriptRoot "appsettings.Development.json"
$rawJson = Get-Content $configPath -Raw
$pattern = '"PublicUrl"\s*:\s*".*?"'
$replacement = "`"PublicUrl`": `"$tunnelUrl`""
$updatedJson = $rawJson -replace $pattern, $replacement
Set-Content -Path $configPath -Value $updatedJson
Write-Host "✅ PublicUrl patched"

# Launch Stripe CLI listener
Start-Process stripe -ArgumentList "listen --forward-to $($tunnelUrl)/api/webhooks/stripe" -WindowStyle Hidden
Write-Host "💳 Stripe CLI listening in background"