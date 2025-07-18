Write-Host "🛑 Stopping OmniTenant Dev Environment..."

# Kill ngrok
Get-Process ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Write-Host "🔌 ngrok terminated"

# Kill stripe CLI (rough match)
Get-Process stripe -ErrorAction SilentlyContinue | Stop-Process -Force
Write-Host "💳 Stripe CLI terminated"