using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganss.Xss;
using Markdig;

namespace SkillProof.Logic.Helper
{
    public class MarkdownService : IMarkdownService
    {
        private readonly HtmlSanitizer _sanitizer;

        public MarkdownService()
        {             
            _sanitizer = new HtmlSanitizer();
        }

        public string ToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            var html = Markdown.ToHtml(markdown);
            return _sanitizer.Sanitize(html);
        }
    }
}
