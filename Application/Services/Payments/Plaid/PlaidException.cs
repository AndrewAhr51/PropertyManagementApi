using Going.Plaid.Entity;

namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public class PlaidException : ApplicationException
    {
        public PlaidError? PlaidError { get; }

        public PlaidException(string message, PlaidError? plaidError = null)
            : base(message)
        {
            PlaidError = plaidError;
        }
    }
}