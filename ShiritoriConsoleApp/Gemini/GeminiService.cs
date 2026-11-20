using Microsoft.Extensions.AI;
using Mscc.GenerativeAI.Microsoft;
using ShiritoriConsoleApp.Models;


namespace ShiritoriConsoleApp.Gemini
{
    public class GeminiService : IGeminiService
    {
        private readonly IChatClient _chatClient;
        public GeminiService()
        {
            string? apiKey =
                Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "GOOGLE_API_KEY was not found");
            }

            _chatClient = new GeminiChatClient(
                apiKey: apiKey,
                model: "gemini-3.6-flash");
        }

        public async Task<string> AskAsync(string prompt)
        {
            var response = await _chatClient.GetResponseAsync(prompt);

            return response.Text;
        }
    }
}
