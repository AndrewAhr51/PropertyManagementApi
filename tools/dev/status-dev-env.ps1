Write-Host "📊 OmniTenant Dev Environment Status Check..."

# 1. Check if ngrok is running
$ngrok = Get-Process ngrok -ErrorAction SilentlyContinue
if ($ngrok) {
    Write-Host "✅ ngrok is running (PID: $($ngrok.Id))"
} else {
    Write-Host "❌ ngrok is not running"
}

# 2. Get tunnel URL
try {
    $tunnels = (Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels" -ErrorAction Stop).tunnels
    $tunnelUrl = $tunnels | Where-Object { $_.public_url -like "https*" } | Select-Object -First 1 | ForEach-Object { $_.public_url }
    if ($tunnelUrl) {
        Write-Host "🔗 ngrok tunnel active: $tunnelUrl"

        # 3. Ping success page
        try {
            $response = Invoke-WebRequest -Uri "$tunnelUrl/payment-success" -UseBasicParsing -Method GET -ErrorAction Stop
            Write-Host "✅ Success page reachable (HTTP $($response.StatusCode))"
        } catch {
            Write-Host "⚠️ Could not reach /payment-success: $($_.Exception.Message)"
        }
    } else {
        Write-Host "❌ No HTTPS tunnel found"
    }
} catch {
    Write-Host "❌ ngrok tunnel API not responding"
}

# 4. Check if Stripe listener is active
$stripe = Get-Process stripe -ErrorAction SilentlyContinue
if ($stripe) {
    Write-Host "✅ Stripe CLI is listening (PID: $($stripe.Id))"
} else {
    Write-Host "❌ Stripe CLI is not running"
}