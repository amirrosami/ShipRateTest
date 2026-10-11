using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class ContainerService : IContainerService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        public ContainerService(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }
        public async Task<Guid> CreateContainer(ContainerFormVM vm)
        {
            var entity = new Container
            {
                Id = Guid.CreateVersion7(),
                ContainerNumber = vm.ContainerNumber,
                ContainerSizeTypeId = vm.ContainerSizeTypeId,
                CntOwnTypeId = vm.CntOwnTypeId,
                OwnLeaseTypeId = vm.OwnLeaseTypeId,
                MaxPayLoad = vm.MaxPayLoad,
                TareWeight = vm.TareWeight,
                OwnerId = vm.OwnerId,
                ResponsibilityByUs = vm.ResponsibilityByUs,
                ResponsibleCompanyId = vm.ResponsibleCompanyId,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };
            _db.Containers.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<Container> GetContainerAsync(Guid id)
        {
            return await _db.Containers.FindAsync(id);
        }
        public async Task<Guid> GetContainerIdAsync(string containerNumber)
        {
            var container = await _db.Containers.SingleOrDefaultAsync(c => c.ContainerNumber == containerNumber && !c.IsDeleted);
            if (container == null) return Guid.Empty;
            return container.Id;
        }
        public async Task<Guid> GetLastOpenCycleContainerUseIdForContainer(Guid containerId)
        {
            var cuse = await _db.ContainerUses.Where(x => x.ContainerId == containerId).OrderByDescending(x => x.CycleStartDate).FirstOrDefaultAsync();
            if (cuse == null) return Guid.Empty;
            return cuse.Id;
        }
    }
}
