namespace ShiritoriConsoleApp.Gemini
{
    internal interface IGeminiService
    {
        Task<string> AskAsync(string prompt);
    }
}
