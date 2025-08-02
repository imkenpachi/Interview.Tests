namespace OrderService.Configurations
{
    public interface IServiceConfiguration
    {
        ConnectionStringCollection ConnectionStrings { get; set; }
        ExternalServiceSettings ExternalService { get; set; }
        PolicySettings PolicySettings { get; set; }
        MessageQueueSettings MessageQueueSettings { get; set; }
    }
}