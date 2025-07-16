using Microsoft.AspNetCore.SignalR;

namespace PropertyManagementAPI.Hubs
{
    public class PaymentsHub: Hub
    {
        public async Task BroadcastInvoiceUpdate(int invoiceId)
        {
            await Clients.All.SendAsync("InvoiceStatusUpdated", invoiceId);
        }
    }
}
