using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("medical_result_images")]
    public class MedicalResultImage : BaseEntity
    {
        [Required]
        public Guid MedicalResultId { get; set; }

        [Required]
        [MaxLength(2048)]
        public string ImageUrl { get; set; } = null!;

        // Navigation
        public MedicalResult MedicalResult { get; set; } = null!;
    }
}
