namespace PropertyManagementAPI.Domain.DTOs.Stripe
{
    public class StripeQueueHealthDto
    {
        public int FallbackQueueCount { get; set; }
        public bool ChannelReady { get; set; }
        public bool IsHealthy => FallbackQueueCount < 50 && ChannelReady;
    }

}
