namespace OrderService.Configurations
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public virtual ConnectionStringCollection ConnectionStrings { get; set; }
        public virtual ExternalServiceSettings ExternalService { get; set; }
        public virtual PolicySettings PolicySettings { get; set; }
        public virtual MessageQueueSettings MessageQueueSettings { get; set; }
    }
}
