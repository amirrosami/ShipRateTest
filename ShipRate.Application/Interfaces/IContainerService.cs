using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.ContainerOperation;

namespace ShipRate.Application.Interfaces
{
    public interface IContainerService
    {
        Task<Guid> CreateContainer(ContainerFormVM container);
        Task<Guid> GetContainerIdAsync(string containerNumber);
        Task<Container> GetContainerAsync(Guid id);
        Task<Guid> GetLastOpenCycleContainerUseIdForContainer(Guid containerId);
    }
}
