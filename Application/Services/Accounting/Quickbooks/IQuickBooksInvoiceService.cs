using Intuit.Ipp.Data;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public interface IQuickBooksInvoiceService
    {
        /// <summary>
        /// Creates an invoice in QuickBooks Online for the specified tenant realm.
        /// </summary>
        /// <param name="realmId">QuickBooks company realm ID</param>
        /// <param name="customer">QuickBooks customer object</param>
        /// <param name="amount">Total amount to invoice</param>
        /// <param name="itemId">QuickBooks item reference ID</param>
        /// <returns>The created QuickBooks invoice object</returns>
        Task<Invoice> CreateInvoiceAsync(string realmId, Customer customer, decimal amount, string itemId);

        /// <summary>
        /// Optional health check for verifying connectivity to QuickBooks for a given realm.
        /// </summary>
        /// <param name="realmId">QuickBooks company identifier</param>
        /// <returns>True if connection is healthy; false otherwise</returns>
        Task<bool> VerifyConnectionAsync(string realmId);
    }
}