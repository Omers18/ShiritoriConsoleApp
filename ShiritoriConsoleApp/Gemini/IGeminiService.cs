using ShiritoriConsoleApp.Models;

namespace ShiritoriConsoleApp.Gemini
{
    public interface IGeminiService
    {
        Task<string> AskAsync(string prompt);
    }
}
