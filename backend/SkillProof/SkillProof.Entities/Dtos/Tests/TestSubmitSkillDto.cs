using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Tests
{
    public class TestSubmitSkillDto
    {
        public string SkillId { get; set; } = string.Empty;
        public List<TestAnswerSubmitDto> Answers { get; set; } = new();
    }
}
