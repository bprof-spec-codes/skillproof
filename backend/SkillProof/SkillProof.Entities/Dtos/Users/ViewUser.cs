using SkillProof.Entities.Dtos.Skill;

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

        public List<SkillViewDto>? Skills { get; set; }
        public List<string> SavedJobIds { get; set; } = new List<string>();
        public List<string> AppliedJobIds { get; set; } = new List<string>();
        public List<BadgeDto> Badges { get; set; } = new List<BadgeDto>();
    }
}
