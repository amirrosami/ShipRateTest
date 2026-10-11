using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class MeasurementUnitDTO
    {
        public int Id { get; set; }
        public int MeasurementUnitCategoryId { get; set; }
        public string MeasurementUnitCategoryName { get; set; }
        public string MeasurementUnitName { get; set; }
        public string MeasurementUnitLocalName { get; set; }
        public bool SystemDefined { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class MeasurementUnitCreateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Unit Name is required.")]
        [StringLength(45, ErrorMessage = "Unit Name cannot exceed {0} characters.")]
        public string UnitName { get; set; }

        [Required(ErrorMessage = "Unit Local Name is required.")]
        [StringLength(45, ErrorMessage = "Unit Local Name cannot exceed {0} characters.")]
        public string UnitLocalName { get; set; }
        public bool IsActive { get; set; } 

    }
    public class MeasurementUnitUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Unit Name is required.")]
        [StringLength(45, ErrorMessage = "Unit Name cannot exceed {0} characters.")]
        public string UnitName { get; set; }

        [Required(ErrorMessage = "Unit Local Name is required.")]
        [StringLength(45, ErrorMessage = "Unit Local Name cannot exceed {0} characters.")]
        public string UnitLocalName { get; set; }

        public bool IsActive { get; set; }
    }
}
