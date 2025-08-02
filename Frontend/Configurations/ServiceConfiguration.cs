namespace Frontend.Configurations
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public virtual ExternalServiceSettings ExternalService { get; set; }
        public virtual PolicySettings PolicySettings { get; set; }
    }
}
