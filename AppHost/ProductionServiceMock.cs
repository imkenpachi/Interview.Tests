using WireMock.Client.Builders;

namespace AppHost
{
    internal class ProductionServiceMock
    {
        public static async Task Build(AdminApiMappingBuilder builder)
        {
            builder.Given(builder => builder
                .WithRequest(request => request
                    .UsingPost()
                    .WithPath("/api/internal/v1/productions")
                )
                .WithResponse(response => response
                    .WithStatusCode(200).WithDelay(TimeSpan.FromSeconds(1))
                )
            );
            await builder.BuildAndPostAsync();
        }
    }
}
