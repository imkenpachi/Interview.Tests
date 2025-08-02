namespace PaymentService.Configurations
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public virtual ConnectionStringCollection ConnectionStrings { get; set; }
        public virtual ExternalService ExternalService { get; set; }
        public virtual PolicySettings PolicySettings { get; set; }
        public virtual string PublicUrl { get; set; }
    }
}
