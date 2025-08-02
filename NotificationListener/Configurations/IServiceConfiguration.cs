namespace NotificationListener.Configurations
{
    public interface IServiceConfiguration
    {
        ConnectionStringCollection ConnectionStrings { get; set; }
        ExternalService ExternalService { get; set; }
        PolicySettings PolicySettings { get; set; }
        MessageQueueSettings MessageQueueSettings { get; set; }
    }
}