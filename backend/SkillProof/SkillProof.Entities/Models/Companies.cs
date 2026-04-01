using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;

namespace SkillProof.Entities.Models
{
    public class Companies: IIdentity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        public string Website { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Job>? Jobs { get; set; }
        public virtual ICollection<Users>? Users { get; set; }
    }
}
