using ShiritoriConsoleApp.Gemini;
using ShiritoriConsoleApp.Models;

namespace ShiritoriConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string answer = """
    {"status":"ok","correctedWord":"apple","replyWord":"elephant","explanation":"An animal"}
    """;
            List<string> answers = new List<string>();
            answers.Add(answer);
            FakeGeminiService fake = new FakeGeminiService(answers);

            Shiritori game = new Shiritori(fake);

            MoveResult result = await game.PlayAsync("apple");

            Console.WriteLine($"Outcome: {result.Outcome}");
            Console.WriteLine($"History count: {game.History.Count}");
            Console.WriteLine($"State: {game.State}");
        }
    }
}