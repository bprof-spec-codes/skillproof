using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Helper
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}
