namespace PropertyManagementAPI.Application.Exceptions
{
    public class PlaidException : ApplicationException
    {
        public string? PlaidErrorCode { get; }

        public PlaidException(string message, string? errorCode = null)
            : base(message)
        {
            PlaidErrorCode = errorCode;
        }
    }
}