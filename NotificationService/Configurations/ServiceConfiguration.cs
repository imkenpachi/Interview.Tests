namespace NotificationService.Configurations
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public virtual ConnectionStringCollection ConnectionStrings { get; set; }
        public virtual ExternalService ExternalService { get; set; }
        public virtual PolicySettings PolicySettings { get; set; }
        public virtual AppSettings AppSettings { get; set; }
    }
}
