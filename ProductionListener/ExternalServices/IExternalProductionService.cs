using ProductionListener.Models.DTOs;

namespace ProductionListener.ExternalServices
{
    public interface IExternalProductionService
    {
        Task SendProductionRequestAsync(Guid userId, ProductionRequest productionRequest);
    }
}
