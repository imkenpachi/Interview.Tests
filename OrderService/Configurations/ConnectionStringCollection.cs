namespace OrderService.Configurations
{
    public class ConnectionStringCollection
    {
        public virtual required string OrderService { get; set; }
        public virtual required string AWSRegion { get; set; }
    }
}
