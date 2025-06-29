# Configuration Setup

## Important Security Notice
For security reasons, `appsettings.json` has been removed from the repository as it contained sensitive information including database credentials, API keys, and other secrets.

## Setup Instructions

1. **Copy the example configuration:**
   ```bash
   cp appsettings.example.json appsettings.json
   ```

2. **Update the configuration with your actual values:**
   - Replace `YOUR_SERVER`, `YOUR_DATABASE`, `YOUR_USERNAME`, `YOUR_PASSWORD` with your actual database connection details
   - Replace `YOUR-VERY-LONG-SECRET-KEY-AT-LEAST-32-BYTES-MINIMUM` with a secure JWT secret key
   - Replace PayPal credentials with your actual PayPal sandbox/production credentials
   - Update SMTP settings with your email server configuration
   - Set appropriate encryption keys

3. **Never commit appsettings.json:**
   The file is now included in `.gitignore` to prevent accidental commits of sensitive data.

## Alternative Configuration Methods

You can also use:
- Environment variables
- Azure Key Vault (for production)
- User secrets for development: `dotnet user-secrets init`

## Development
The application will fall back to `appsettings.Development.json` and environment variables if `appsettings.json` is not present.