namespace NotificationService.Configurations
{
    public class ConnectionStringCollection
    {
        public virtual required string NotificationService { get; set; }
        public virtual required string AWSRegion { get; set; }
    }
}
