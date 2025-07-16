using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    public class DepartmentCreateWithServicesDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }

        [Required]
        public string ServicesJson { get; set; }
    }

    public class DepartmentUpdateWithServicesDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required]
        public string ServicesJson { get; set; }
    }

}
