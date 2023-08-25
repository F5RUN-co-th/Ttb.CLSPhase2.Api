using System.ComponentModel.DataAnnotations;

namespace CLSPhase2.Api.Models
{
    public abstract class BaseModel
    {
        [Required]
        public long entityId { get; set; }
    }
}
