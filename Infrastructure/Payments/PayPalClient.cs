using PayPalCheckoutSdk.Core;
using Microsoft.Extensions.Configuration;

namespace PropertyManagementAPI.Infrastructure.Payments
{

    public class PayPalClient
    {
        private readonly string _clientId;
        private readonly string _secret;

        public PayPalClient(string clientId, string secret)
        {
            _clientId = clientId;
            _secret = secret;
        }

        public PayPalHttpClient Client()
        {
            var environment = new SandboxEnvironment(_clientId, _secret);
            return new PayPalHttpClient(environment);
        }

    }
}