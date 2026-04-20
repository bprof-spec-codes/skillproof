using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Logic.Helper
{
    public interface IMarkdownService
    {
        string ToHtml(string markdown);
    }
}
