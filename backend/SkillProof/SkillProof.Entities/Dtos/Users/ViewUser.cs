namespace SkillProof.Entities.Dtos.Users
{
    public class ViewUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string? CompanyId { get; set; }
    }
}
