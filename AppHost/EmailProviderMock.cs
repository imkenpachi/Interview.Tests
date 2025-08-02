using WireMock.Client.Builders;

namespace AppHost
{
    internal class EmailProviderMock
    {
        public static async Task Build(AdminApiMappingBuilder builder)
        {
            builder.Given(builder => builder
                .WithRequest(request => request
                    .UsingPost()
                    .WithPath("/api/v1/transactional-emails")
                )
                .WithResponse(response => response
                    .WithStatusCode(200)
                    .WithBodyAsJson(new SendEmailNotificationResponse { TrackingId = Guid.NewGuid() })
                    .WithDelay(TimeSpan.FromSeconds(1))
                )
            );

            await builder.BuildAndPostAsync();
        }
    }

    record SendEmailNotificationResponse()
    {
        public required Guid TrackingId { get; set; }
    }
}
