using System.ComponentModel.DataAnnotations;
using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Job;

public class JobCreateDto
{
    
    [Required]
    public string CompanyId { get; set; }
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Location { get; set; }

    public string Tags { get; set; }

    [Required]
    public EmploymentType EmploymentType { get; set; }
}