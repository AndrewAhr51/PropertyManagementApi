# Property Management API

## Setup and Configuration

### Configuration File Setup

For security reasons, the `appsettings.json` file is not included in the repository as it contains sensitive information like database connection strings, JWT secret keys, and SMTP credentials.

To set up the application:

1. Copy the example configuration file:
   ```bash
   cp appsettings.json.example appsettings.json
   ```

2. Edit `appsettings.json` and replace the placeholder values with your actual configuration:
   - **ConnectionStrings**: Update database connection strings with your actual server details and credentials
   - **JwtSettings.SecretKey**: Use a secure secret key (minimum 32 bytes)
   - **SmtpSettings**: Configure your SMTP server details for email functionality
   - **EncryptionSettings**: Set up encryption keys for data protection

### Required Configuration Sections

- `ConnectionStrings`: Database connection configurations
- `JwtSettings`: JWT authentication settings
- `RoleSettings`: User role configurations
- `CorsPolicy`: Cross-origin resource sharing settings
- `SmtpSettings`: Email server configuration
- `EncryptionSettings`: Data encryption configuration

### Security Notes

- Never commit `appsettings.json` to version control
- Use environment-specific configuration files for different deployments
- Store sensitive values in secure configuration providers or environment variables in production
- Regularly rotate secret keys and passwords

## Building and Running

```bash
dotnet build
dotnet run
```

The application will warn you if `appsettings.json` is missing and guide you to copy from the example file.