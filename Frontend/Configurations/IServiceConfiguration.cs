namespace Frontend.Configurations
{
    public interface IServiceConfiguration
    {
        ExternalServiceSettings ExternalService { get; set; }
        PolicySettings PolicySettings { get; set; }
    }
}