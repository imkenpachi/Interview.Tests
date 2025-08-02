namespace NotificationService.Configurations
{
    public interface IServiceConfiguration
    {
        ConnectionStringCollection ConnectionStrings { get; set; }
        ExternalService ExternalService { get; set; }
        PolicySettings PolicySettings { get; set; }
        AppSettings AppSettings { get; set; }
    }
}