try {
    Write-Host "🚀 Launching OmniTenant Stripe Dev Environment..."

    # 1. Start ngrok tunnel
    Start-Process ngrok -ArgumentList "http https://localhost:7144"
    Start-Sleep -Seconds 3

    # 2. Detect the HTTPS tunnel
    $tunnelUrl = $null
    for ($i = 0; $i -lt 5; $i++) {
        try {
            $tunnels = (Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels" -ErrorAction Stop).tunnels
            $tunnelUrl = $tunnels | Where-Object { $_.public_url -like "https*" } | Select-Object -First 1 | ForEach-Object { $_.public_url }
            if ($tunnelUrl) { break }
        } catch { Start-Sleep -Seconds 2 }
    }

    if (-not $tunnelUrl) { throw "❌ No active HTTPS ngrok tunnel detected." }
    Write-Host "🔗 HTTPS tunnel detected: $tunnelUrl"

    # 3. Patch appsettings.Development.json with PublicUrl
    $configPath = ".\appsettings.Development.json"
    if (!(Test-Path $configPath)) { throw "Missing config file: $configPath" }

    $rawJson = Get-Content $configPath -Raw
    $pattern = '"PublicUrl"\s*:\s*".*?"'
    $replacement = "`"PublicUrl`": `"$tunnelUrl`""
    $updatedJson = $rawJson -replace $pattern, $replacement
    Set-Content -Path $configPath -Value $updatedJson
    Write-Host "✅ PublicUrl updated to current tunnel"

    # 4. Start Stripe CLI webhook forwarding (non-blocking)
    Start-Process stripe -ArgumentList "listen --forward-to $($tunnelUrl)/api/webhooks/stripe"
    Write-Host "💳 Stripe CLI forwarding launched in background"

    # 5. Verify payment success page is routable
    try {
        $response = Invoke-WebRequest -Uri "$tunnelUrl/payment-success" -UseBasicParsing -Method GET -ErrorAction Stop
        Write-Host "✅ Payment success page is reachable (HTTP $($response.StatusCode))"
    } catch {
        Write-Host "⚠️ Redirect target may not be live: $($_.Exception.Message)"
    }
}
catch {
    Write-Host "💥 Dev bootstrap failed: $($_.Exception.Message)"
}
finally {
    exit 0
}