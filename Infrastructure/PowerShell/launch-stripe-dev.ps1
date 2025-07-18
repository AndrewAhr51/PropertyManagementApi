try {
    Start-Process ngrok -ArgumentList "http https://localhost:7144" -WindowStyle Hidden -PassThru
    Start-Sleep -Seconds 3

    $response = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels" -ErrorAction Stop
    if ($response.tunnels.Count -eq 0 -or !$response.tunnels[0].public_url) {
        throw "No active ngrok tunnel found."
    }

    $tunnelUrl = $response.tunnels[0].public_url
    Write-Host "🔗 ngrok tunnel active at: $tunnelUrl"

    $jsonPath = ".\appsettings.Development.json"
    if (!(Test-Path $jsonPath)) {
        throw "Missing configuration file: $jsonPath"
    }

    $rawJson = Get-Content $jsonPath -Raw
    $updatedJson = $rawJson -replace '"PublicUrl"\s*:\s*".*?"', '"PublicUrl": "' + $tunnelUrl + '"'
    Set-Content -Path $jsonPath -Value $updatedJson

    Write-Host "✅ Updated PublicUrl in appsettings.Development.json"

    stripe listen --forward-to "$($tunnelUrl)/api/webhooks/stripe" | Out-Null
    Write-Host "✅ Stripe CLI forwarding active."
}
catch {
    Write-Host "⚠️ Script failed: $($_.Exception.Message)"
}
finally {
    exit 0
}