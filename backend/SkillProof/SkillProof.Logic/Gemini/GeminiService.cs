using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using SkillProof.Entities.Models.Gemini;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkillProof.Logic.Gemini
{
    public class GeminiService : IGeminiService
    {
        private readonly Client _client;
        private readonly IConfiguration _config;
        private static readonly SemaphoreSlim _rateLimiter = new SemaphoreSlim(1, 1);

        public GeminiService(IConfiguration config)
        {
            string apiKey = config["Gemini:ApiKey"];
            _client = new Client(apiKey: apiKey);
        }

        public async Task<double> EvaluateAnswerAsync(GradingRequest request)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var prompt = $@"
              You are an expert strict exam evaluator. 
              Evaluate the user's answer to the following question. 
              Score the answer as 0 (completely wrong), 0.5 (partially correct), or 1 (perfect).
              Respond ONLY with the number (0, 0.5, or 1). No explanation, no extra text.

              Question: {request.Question}
              Student Answer: {request.StudentAnswer}";

                var config = new GenerateContentConfig
                {
                    ResponseMimeType = "application/json"
                };

                var response = await _client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: prompt,
                    config: config
                );

                await Task.Delay(TimeSpan.FromSeconds(4));

                if (double.TryParse(response.Text, out double parsedScore))
                {
                    return parsedScore;
                }

                return 0.0;
            }
            finally
            {
                _rateLimiter.Release();
            }
        }

        public async Task<ICollection<double>> EvalueateAllAsync(List<GradingRequest> requests)
        {
            var answers = new List<double>();
            foreach (var request in requests)
            {
                double score = await EvaluateAnswerAsync(request);
                answers.Add(score);
            }
            return answers;
        }
    }
}

    
