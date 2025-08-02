namespace PaymentService.Configurations
{
    public interface IServiceConfiguration
    {
        ConnectionStringCollection ConnectionStrings { get; set; }
        ExternalService ExternalService { get; set; }
        PolicySettings PolicySettings { get; set; }
        string PublicUrl { get; set; }
    }
}