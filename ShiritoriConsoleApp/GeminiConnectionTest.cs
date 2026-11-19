using Microsoft.Extensions.AI;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Microsoft;
using Mscc.GenerativeAI.Types;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ShiritoriConsoleApp
{
    internal class GeminiConnectionTest
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task TestGemini()
        {
            try
            {
                string? apiKey =
                    Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.WriteLine("GOOGLE_API_KEY was not found.");
                    return;
                }

                IChatClient chatClient =
                    new GeminiChatClient(
                        apiKey: apiKey,
                        model: "gemini-3.6-flash");

                Console.Write("Ask Gemini something: ");

                string prompt =
                    Console.ReadLine() ?? "";

                var response =
                    await chatClient.GetResponseAsync(prompt);

                Console.WriteLine();
                Console.WriteLine("--- Gemini's Response ---");
                Console.WriteLine(response.Text);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(
                    $"Unable to reach Gemini: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine(
                    "The Gemini request timed out or was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}