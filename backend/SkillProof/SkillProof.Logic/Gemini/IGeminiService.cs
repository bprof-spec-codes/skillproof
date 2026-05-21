using SkillProof.Entities.Models.Gemini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Logic.Gemini
{
    public interface IGeminiService
    {
        Task<double> EvaluateAnswerAsync(GradingRequest request);
    }
}
