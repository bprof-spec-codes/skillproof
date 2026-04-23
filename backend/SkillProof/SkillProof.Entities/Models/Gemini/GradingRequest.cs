using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Models.Gemini
{
    public class GradingRequest
    {
        public string Question { get; set; }
        public string StudentAnswer { get; set; }
    }
}
