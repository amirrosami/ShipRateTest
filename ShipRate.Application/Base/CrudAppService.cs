using DocumentFormat.OpenXml.InkML;
using FastReport.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;

namespace ShipRate.Application.Base
{

    public class LocationService : CrudAppService<Location, LocationCreateEditDto, LocationCreateEditDto, LocationListDto, Guid>

    {
        public LocationService(IServiceProvider serviceProvider) : base(serviceProvider){}


        protected override IQueryable<LocationListDto> GetBaseFilteredQuery(PagedRequestDto<object> dto)
        {
            throw new NotImplementedException();
        }

        protected override Location MapToEntity(LocationCreateEditDto input)
        {
            throw new NotImplementedException();
        }

        protected override void MapToExistingEntity(LocationCreateEditDto input, Location entity)
        {
            throw new NotImplementedException();
        }

        protected override LocationListDto MapToReadDto(Location entity)
        {
            throw new NotImplementedException();
        }
    }

    public class GeneralException : Exception
    {
        public GeneralException(string message):base(message)
        {
        }
    }

    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public string? ErrorCode { get; set; }


        public ServiceResult(bool isSuccess, List<string> errorMessages, string errorCode)
        {
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
            ErrorCode = errorCode;
        }
        public static ServiceResult Success()
        {
            return new ServiceResult(true,null,null);
        }
        public static ServiceResult Fail(List<string> errorMessages, string errorCode = "")
        {
            return new ServiceResult(false, errorMessages, errorCode);
        }
        public ServiceResult WithMessage(string message)
        {
            if (ErrorMessages == null)
            {
                ErrorMessages = new List<string>();
            }
            ErrorMessages.Add(message);
            return this;
        }
    }

    public enum OperationMode
    {
        Create,Update, Delete
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public object CustomFilterValues { get; set; }

        public PagedResult<object> toObject()
        {
            return new PagedResult<object>
            {
                Items = this.Items.Select(x => (object)x).ToList(),
                TotalCount = this.TotalCount,
                Page = this.Page,
                PageSize = this.PageSize,
                SearchTerm = this.SearchTerm,
                SortDirection = this.SortDirection,
                CustomFilterValues = this.CustomFilterValues,
                SortColumn = this.SortColumn,
            };
        }
    }
    public abstract class CrudAppService<TEntity, TCreateDto, TUpdateDto, TReadDto, TKey>
     where TEntity : BusinessEntity, new()
    {
        protected readonly ApplicationDbContext _dbcontext;
        protected readonly IRepository<TEntity> _repository;
        protected readonly IValidator<TEntity> _entityValidator;
        protected readonly ICurrentUserService _currentUser;
        protected CrudAppService(
            IServiceProvider serviceProvider)
        {
            _repository = serviceProvider.GetRequiredService<IRepository<TEntity>>();
            _entityValidator = serviceProvider.GetService<IValidator<TEntity>>();
            _dbcontext  = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _currentUser = serviceProvider.GetRequiredService<ICurrentUserService>();
        }
        public virtual void SetOtherProperties(TEntity entity ,OperationMode mode)
        {
            return;
        }
        public virtual async Task<ServiceResult> CreateAsync(TCreateDto input)
        {
                var entity = MapToEntity(input);
                SetOtherProperties(entity, OperationMode.Create);
                if (_entityValidator != null)
                {
                    var validationResult = await _entityValidator.ValidateAsync(entity);
                    if (validationResult != null && !validationResult.IsValid)
                    {
                        return ServiceResult.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    }
                }
                var errors = await CheckConsistencyAsync(entity);
                if (errors != null)
                {
                    return ServiceResult.Fail(errors);
                }

            entity.CreateUserId = _currentUser.UserId;
            entity.CreatedAt = DateTime.Now;
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            await OnOperationSuccessAsync(OperationMode.Create, entity);

            return ServiceResult.Success();
        }

        public virtual async Task<ServiceResult> UpdateAsync(Guid id, TUpdateDto input)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null)
                throw new GeneralException("Entity not found");

            MapToExistingEntity(input, entity);
            SetOtherProperties(entity, OperationMode.Update);
            if (_entityValidator != null)
            {
                var validationResult = await _entityValidator.ValidateAsync(entity);
                if (validationResult != null && !validationResult.IsValid)
                {
                    return ServiceResult.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }
            }

            var errors = await CheckConsistencyAsync(entity);
            if (errors !=null)
            {
                return ServiceResult.Fail(errors);
            }
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            await OnOperationSuccessAsync(OperationMode.Update,entity);
            return ServiceResult.Success();
        }

        
        public virtual async Task<TReadDto?> GetAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            return entity == null ? default : MapToReadDto(entity);
        }

        public virtual async Task<TEntity?> GetEntityAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            return entity == null ? default : entity;
        }



        public virtual async Task<List<TReadDto>> GetAllAsListAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToReadDto).ToList();
        }

        protected abstract IQueryable<TReadDto> GetBaseFilteredQuery(PagedRequestDto<object> dto);
        
        private IQueryable<TReadDto> ApplySort(IQueryable<TReadDto> query , PagedRequestDto dto)
        {
            query = query.OrderBy($"{dto.SortColumn} {dto.SortDirection}");
            return query;
        }

        public virtual async Task<PagedResult<TReadDto>> GetPagedResultAsync(PagedRequestDto<object> dto)
        {
            var query = GetBaseFilteredQuery(dto);
            query = ApplySort(query,dto);
            var count = await query.CountAsync();
            query = query.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await query.ToListAsync();

            return new PagedResult<TReadDto>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalCount = count,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection
            };
        }

        public virtual async Task<ServiceResult> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null)
                throw new Exception("Entity not found");

            var errors = await CheckConsistencyAsync(entity);
            if (errors != null)
            {
                return ServiceResult.Fail(errors);
            }
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            await OnOperationSuccessAsync(OperationMode.Delete ,entity);
            return ServiceResult.Success();
        }

        protected abstract TEntity MapToEntity(TCreateDto input);
        protected abstract void MapToExistingEntity(TUpdateDto input, TEntity entity);
        protected abstract TReadDto MapToReadDto(TEntity entity);
        public virtual async Task OnOperationSuccessAsync(OperationMode mode, TEntity entity)
        {
            return;
        }
        public virtual async Task<List<string>> CheckConsistencyAsync(TEntity entity)
        {
            return null;
        }
    }

}
