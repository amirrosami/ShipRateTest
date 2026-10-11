using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{

    public class ContainerActionSearchFormVm
    {
        [Display(Name = "Container Number")]
        public string? ContainerNumber { get; set; }

        [Display(Name = "Action Type")]
        public int? CntActionTypeId { get; set; }

        [Display(Name = "Location")]
        public Guid? LocationId { get; set; }


        [Display(Name = "Location")]
        public string? LocationName { get; set; }



        [Display(Name = "Sub Location")]
        public Guid? SubLocationId { get; set; }

        [Display(Name = "Voyage")]
        public Guid? VoyageId { get; set; }

        [Display(Name = "Voyage Type")]
        public int? VoyageTypeId { get; set; }

        [Display(Name = "Ownership Type")]
        public int? CntOwnTypeId { get; set; }

        [Display(Name = "Full / Empty")]
        public int? FullMTId { get; set; }

        [Display(Name = "Event Source")]
        public int? EventActionSourceId { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<SelectListItem> ActionTypes { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> SubLocations { get; set; } = new();
        public List<SelectListItem> Voyages { get; set; } = new();
        public List<SelectListItem> VoyageTypes { get; set; } = new();
        public List<SelectListItem> OwnershipTypes { get; set; } = new();
        public List<SelectListItem> FullEmptyTypes { get; set; } = new();
        public List<SelectListItem> EventSources { get; set; } = new();
    }

    public class ContainerActionListItemVm
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }

        public Guid ContainerId { get; set; }

        public string ContainerNumber { get; set; } = string.Empty;

        public int CntActionTypeId { get; set; }
        public string ActionTypeName { get; set; } = string.Empty;

        public Guid? LocationId { get; set; }
        public string? LocationName { get; set; }

        public Guid? SubLocationId { get; set; }
        public string? SubLocationName { get; set; }

        public Guid? VoyageId { get; set; }
        public string? VoyageNumber { get; set; }

        public int? VoyageTypeId { get; set; }
        public string? VoyageTypeName { get; set; }

        public int? ContainerActionStatusId { get; set; }
        public string? ContainerActionStatusName { get; set; }



        public int CntOwnTypeId { get; set; }
        public string OwnershipTypeName { get; set; } = string.Empty;

        public int FullMTId { get; set; }
        public string FullEmptyName { get; set; } = string.Empty;

        public DateTime ContainerActionDateTime { get; set; }

        public int EventActionSourceId { get; set; }
        public string EventSourceName { get; set; } = string.Empty;

        public string? ContainerActionSourceReference { get; set; }

        public DateTime CreatedAt { get; set; }
    }
    public class ContainerActionIndexVm
    {
        public ContainerActionSearchFormVm SearchForm { get; set; } = new();

        public PagedResultDto<ContainerActionListItemVm> Result { get; set; } = new();
    }
    public class ReleaseToShipperVm
    {
        [Required]
        [Display(Name = "Container Number")]
        public string ContainerNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Action Date / Time")]
        public DateTime ContainerActionDateTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Location")]
        public Guid? LocationId { get; set; }

        [Display(Name = "Sub Location")]
        public Guid? SubLocationId { get; set; }

        [Display(Name = "Voyage")]
        public Guid? VoyageId { get; set; }

        [Required]
        [Display(Name = "Ownership Type")]
        public int? CntOwnTypeId { get; set; }

        [Required]
        [Display(Name = "Full / Empty")]
        public int? FullMTId { get; set; }

        [Display(Name = "Reference")]
        [StringLength(100)]
        public string? ContainerActionSourceReference { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(500)]
        public string? Remarks { get; set; }

        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> SubLocations { get; set; } = new();
        public List<SelectListItem> Voyages { get; set; } = new();
        public List<SelectListItem> OwnershipTypes { get; set; } = new();
        public List<SelectListItem> FullEmptyTypes { get; set; } = new();
    }



    public class LoadOnBoardVm
    {
        [Required]
        [Display(Name = "Container Number")]
        public string ContainerNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Action Date / Time")]
        public DateTime ContainerActionDateTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Location")]
        public Guid? LocationId { get; set; }
        public string? LocationName { get; set; }

        [Display(Name = "Sub Location")]
        public Guid? SubLocationId { get; set; }
        public string? SubLocationName { get; set; }

        [Display(Name = "Voyage")]
        public Guid? VoyageId { get; set; }

        [Required]
        [Display(Name = "Ownership Type")]
        public int? CntOwnTypeId { get; set; }

        [Required]
        [Display(Name = "Full / Empty")]
        public int? FullMTId { get; set; }

        [Display(Name = "Reference")]
        [StringLength(100)]
        public string? ContainerActionSourceReference { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(500)]
        public string? Remarks { get; set; }

        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> SubLocations { get; set; } = new();
        public List<SelectListItem> Voyages { get; set; } = new();
        public List<SelectListItem> OwnershipTypes { get; set; } = new();
        public List<SelectListItem> FullEmptyTypes { get; set; } = new();
    }


    public class DischargeVm
    {
        [Required]
        [Display(Name = "Container Number")]
        public string ContainerNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Action Date / Time")]
        public DateTime ContainerActionDateTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Location")]
        public Guid? LocationId { get; set; }

        public string? LocationName { get; set; }

        [Display(Name = "Sub Location")]
        public Guid? SubLocationId { get; set; }

        public string? SubLocationName { get; set; }



        [Display(Name = "Voyage")]
        public Guid? VoyageId { get; set; }

        [Required]
        [Display(Name = "Ownership Type")]
        public int? CntOwnTypeId { get; set; }

        [Required]
        [Display(Name = "Full / Empty")]
        public int? FullMTId { get; set; }

        [Display(Name = "Reference")]
        [StringLength(100)]
        public string? ContainerActionSourceReference { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(500)]
        public string? Remarks { get; set; }

        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> SubLocations { get; set; } = new();
        public List<SelectListItem> Voyages { get; set; } = new();
        public List<SelectListItem> OwnershipTypes { get; set; } = new();
        public List<SelectListItem> FullEmptyTypes { get; set; } = new();
    }




    public class RBCVm
    {
        [Required]
        [Display(Name = "Container Number")]
        public string ContainerNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Action Date / Time")]
        public DateTime ContainerActionDateTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Location")]
        public Guid? LocationId { get; set; }
        public string? LocationName { get; set; }

        [Display(Name = "Depot")]
        public Guid? SubLocationId { get; set; }
        public string? SubLocationName { get; set; }


        [Display(Name = "Condition")]
        public int? ContainerConditionId { get; set; }


        [Display(Name = "Reference")]
        [StringLength(100)]
        public string? ContainerActionSourceReference { get; set; }




        [Display(Name = "Remarks")]
        [StringLength(500)]

        public string? Remarks { get; set; }

        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> SubLocations { get; set; } = new();
        public List<SelectListItem> ContainerConditions { get; set; } = new();

    }



    public class ContainerLookupResultViewModel
    {
        public string? ContainerNumber { get; set; }

        /// <summary>
        /// آیا کانتینر در دیتابیس وجود دارد؟
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// شناسه کانتینر در صورت موجود بودن
        /// </summary>
        public Guid? ContainerId { get; set; }

        /// <summary>
        /// عنوان نوع/سایز کانتینر موجود (مثلاً 20DV, 40HC)
        /// </summary>
        public string? SizeTypeName { get; set; }

        /// <summary>
        /// شناسه نوع کانتینر
        /// </summary>
        public Guid? ContainerSizeTypeId { get; set; }

        /// <summary>
        /// وزن خالی ثبت‌شده در سیستم (کیلوگرم)
        /// </summary>
        public decimal? TareWeight { get; set; }

        /// <summary>
        /// حداکثر ظرفیت بار (کیلوگرم)
        /// </summary>
        public decimal? MaxPayload { get; set; }

        /// <summary>
        /// وضعیت فعلی کانتینر در دپو/سیستم (مثلاً Available, Damaged, In-Use)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// آیا نوع کانتینر پیدا شده با نوع درخواستی RTS مغایرت دارد؟
        /// </summary>
        public bool IsMismatchWithType { get; set; }

        /// <summary>
        /// لیست انتخابی نوع کانتینرها جهت نمایش دراپ‌داون در زمان عدم وجود کانتینر
        /// </summary>
        public SelectList? ContainerTypeSelectList { get; set; }
    }
    public class ContainerFormVM
    {
        public Guid? Id { get; set; }

        [Required, MaxLength(12)]
        public string ContainerNumber { get; set; }

        [Required]
        public int ContainerSizeTypeId { get; set; }

        [Required]
        public int CntOwnTypeId { get; set; }

        [Required]
        public int OwnLeaseTypeId { get; set; }

        [Required]
        public int MaxPayLoad { get; set; }

        [Required]
        public int TareWeight { get; set; }

        public Guid? OwnerId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool ResponsibilityByUs { get; set; }
        public int? ResponsibleCompanyId { get; set; }
    }



}



