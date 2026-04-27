using Microsoft.AspNetCore.Identity;
using SkillProof.Entities.Helper;
using System.ComponentModel.DataAnnotations;

namespace SkillProof.Entities.Models
{
    public class Users: IdentityUser, IIdentity
    {
        // The Id, Username, Email etc. are all comes from the IdentityUser class,
        // which is the base class for the user entity in ASP.NET Core Identity.

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public byte[] ProfilePicture { get; set; }

        public string Headline { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyRole { get; set; }      
        public string? Skills { get; set; } = null;
        public virtual Companies Companies { get; set; }
        public virtual ICollection<UserExperiences>? UserExperiences { get; set; }
        public virtual ICollection<Tests>? Tests { get; set; }
        public virtual ICollection<JobApplication>? JobApplications { get; set; }

        public Users()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
