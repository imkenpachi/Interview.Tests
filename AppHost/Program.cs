using Amazon;
using AppHost;
using Aspire.Hosting.LocalStack;

var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sql-server", port: 2433)
                 .WithLifetime(ContainerLifetime.Persistent);
var orderServiceDb = sqlserver.AddDatabase("OrderService", "order-service");
var paymentServiceDb = sqlserver.AddDatabase("PaymentService", "payment-service");
var notificationServiceDb = sqlserver.AddDatabase("NotificationService", "notification-service");

var emailProviderMock = builder.AddWireMockNet("mock-email-provider")
    .WithApiMappingBuilder(EmailProviderMock.Build);

var productionServiceMock = builder.AddWireMockNet("mock-production-service")
    .WithApiMappingBuilder(ProductionServiceMock.Build);

var invoiceServiceMock = builder.AddWireMockNet("mock-invoice-service")
    .WithApiMappingBuilder(InvoiceServiceMock.Build);

var ewalletProviderMock = builder.AddProject<Projects.EwalletProviderMock>("mock-ewallet-provider");

#region LocalStack
var regionEndpoint = RegionEndpoint.APSoutheast1;
var awsConfig = builder.AddAWSSDKConfig().WithProfile("default").WithRegion(regionEndpoint);
// Set up a configuration for the LocalStack
var localStackOptions = builder.AddLocalStackConfig().WithRegion(regionEndpoint.SystemName);

// Bootstrap the localstack container with enhanced configuration
var localstack = builder
    .AddLocalStack("localstack", localStackOptions, container =>
    {
        container.Lifetime = ContainerLifetime.Session;
        container.DebugLevel = 1;
        container.LogLevel = LocalStackLogLevel.Debug;
    });

// Provision application level resources like SQS queues and SNS topics defined in the CloudFormation template file app-resources.template.
var awsResources = builder.AddAWSCloudFormationTemplate("LocalStackAWSResource", "app-resources.template")
    .WithParameter("DefaultVisibilityTimeout", "30")
    // Add the SDK configuration so the AppHost knows what account/region to provision the resources.
    .WithReference(awsConfig)
    // Add the LocalStack configuration
    .WithLocalStack(localstack)
    .WaitFor(localstack);

awsResources.WithTag("aws-repo", "integrations-on-dotnet-aspire-for-aws");

#endregion

var paymentService = builder.AddProject<Projects.PaymentService>("svc-payment")
    .WithExternalHttpEndpoints()
    .WithReference(paymentServiceDb)
    .WaitFor(paymentServiceDb)
    .WithReference(ewalletProviderMock)
    .WaitFor(ewalletProviderMock)
    .WithEnvironment("ExternalService__EwalletProvider", ewalletProviderMock.GetEndpoint("https"));

var orderService = builder.AddProject<Projects.OrderService>("svc-order")
    .WithExternalHttpEndpoints()
    .WithReference(orderServiceDb)
    .WaitFor(orderServiceDb)
    .WithReference(awsResources)
    .WithReference(localstack)
    .WithEnvironment("ConnectionStrings__ProductionServiceSender", awsResources.GetOutput("RequestProductionQueueUrl"))
    .WithEnvironment("ConnectionStrings__InvoiceServiceSender", awsResources.GetOutput("GenerateInvoiceQueueUrl"))
    .WithEnvironment("ConnectionStrings__NotificationServiceSender", awsResources.GetOutput("SendNotificationQueueUrl"))
    .WithEnvironment("ExternalService__PaymentService", paymentService.GetEndpoint("https"));

var notificationService = builder.AddProject<Projects.NotificationService>("svc-notification")
    .WithExternalHttpEndpoints()
    .WithReference(notificationServiceDb)
    .WaitFor(notificationServiceDb)
    .WithReference(emailProviderMock)
    .WaitFor(emailProviderMock)
    .WithEnvironment("ExternalService__EmailProvider", emailProviderMock.GetEndpoint("http"));

paymentService
    .WithEnvironment("PublicUrl", paymentService.GetEndpoint("https"));

paymentService
    .WithEnvironment("ExternalService__OrderService", orderService.GetEndpoint("https"));

builder.AddProject<Projects.Frontend>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(orderService)
    .WaitFor(orderService)
    .WithEnvironment("ExternalService__OrderService", orderService.GetEndpoint("https"));

builder.AddProject<Projects.InvoiceListener>("listener-invoice")
    .WithReference(awsResources)
    .WithReference(localstack)
    .WaitFor(awsResources)
    .WaitFor(invoiceServiceMock)
    .WithEnvironment("ConnectionStrings__InvoiceServiceSender", awsResources.GetOutput("GenerateInvoiceQueueUrl"))
    .WithEnvironment("ExternalService__OrderService", orderService.GetEndpoint("https"))
    .WithEnvironment("ExternalService__InvoiceService", invoiceServiceMock.GetEndpoint("http"));


builder.AddProject<Projects.ProductionListener>("listener-production")
    .WithReference(awsResources)
    .WithReference(localstack)
    .WaitFor(awsResources)
    .WaitFor(invoiceServiceMock)
    .WithEnvironment("ConnectionStrings__ProductionServiceSender", awsResources.GetOutput("RequestProductionQueueUrl"))
    .WithEnvironment("ExternalService__OrderService", orderService.GetEndpoint("https"))
    .WithEnvironment("ExternalService__ProductionService", productionServiceMock.GetEndpoint("http"));

builder.AddProject<Projects.NotificationListener>("listener-notification")
    .WithReference(awsResources)
    .WithReference(localstack)
    .WaitFor(awsResources)
    .WaitFor(invoiceServiceMock)
    .WithEnvironment("ConnectionStrings__NotificationServiceSender", awsResources.GetOutput("SendNotificationQueueUrl"))
    .WithEnvironment("ExternalService__OrderService", orderService.GetEndpoint("https"))
    .WithEnvironment("ExternalService__NotificationService", notificationService.GetEndpoint("https"));

builder.Build().Run();
