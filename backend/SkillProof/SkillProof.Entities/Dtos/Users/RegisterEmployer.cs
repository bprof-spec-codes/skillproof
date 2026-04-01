namespace SkillProof.Entities.Dtos.Users;

public class RegisterEmployer
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string CompanyName { get; set; }
    public string CompanyDescription { get; set; }
    public string? CompanyWebsite { get; set; }
}