using System.ComponentModel.DataAnnotations;
using SkillProof.Entities.Enums;

namespace SkillProof.Entities.Dtos.Job;

public class JobCreateDto
{
    
    public string Title { get; set; }
    public string Location { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public string? Salary { get; set; }
    public string Description { get; set; }
    public string Tags { get; set; }
}