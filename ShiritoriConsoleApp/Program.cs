using ShiritoriConsoleApp.Gemini;
using ShiritoriConsoleApp.Models;

namespace ShiritoriConsoleApp
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            // RunFakeServiceChecks();

            IGeminiService service = new GeminiService();
            Shiritori game = new Shiritori(service);

            PrintWelcome();

            while (true)
            {
                // One game
                while (game.State == GameState.InProgress)
                {
                    Console.WriteLine();

                    if (game.History.Count == 0)
                    {
                        Console.WriteLine("Opening turn: enter any valid English word.");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Enter an English word starting with '{game.RequiredLetter}'.");
                    }

                    Console.Write("Your word: ");
                    string playerWord = Console.ReadLine() ?? "";

                    try
                    {
                        MoveResult result = await game.PlayAsync(playerWord);
                        MoveOutcome outcome = result.Outcome;

                        Console.WriteLine();

                        if (outcome == MoveOutcome.Accepted)
                        {
                            Console.WriteLine("Word accepted!");

                            if (result.PlayerTurn != null &&
                                result.PlayerTurn.WasCorrected)
                            {
                                Console.WriteLine(
                                    $"Spelling corrected: {result.PlayerTurn.Word}");
                            }

                            Console.WriteLine();

                            if (result.GeminiTurn != null)
                            {
                                Console.WriteLine(
                                    $"Gemini's reply: {result.GeminiTurn.Word}");

                                if (!string.IsNullOrWhiteSpace(
                                    result.GeminiTurn.Explanation))
                                {
                                    Console.WriteLine(
                                        $"Explanation: {result.GeminiTurn.Explanation}");
                                }
                            }
                        }
                        else if (outcome == MoveOutcome.PlayerWins)
                        {
                            Console.WriteLine("Congratulations! You won!");
                            Console.WriteLine(
                                "Gemini couldn't find a valid reply.");

                            if (result.PlayerTurn != null &&
                                result.PlayerTurn.WasCorrected)
                            {
                                Console.WriteLine(
                                    $"Your corrected word: {result.PlayerTurn.Word}");
                            }
                        }
                        else if (outcome == MoveOutcome.NotAWord)
                        {
                            Console.WriteLine("Game over! Gemini wins.");
                            Console.WriteLine(
                                "Your word was not recognized as an English word.");
                        }
                        else
                        {
                            if (outcome == MoveOutcome.NotWellFormed)
                            {
                                Console.WriteLine(
                                    "Invalid input. Use English letters only.");
                            }
                            else if (outcome == MoveOutcome.WrongLetter)
                            {
                                Console.WriteLine(
                                    $"Wrong starting letter. Use '{game.RequiredLetter}'.");
                            }
                            else if (outcome == MoveOutcome.AlreadyUsed)
                            {
                                Console.WriteLine(
                                    "This word has already been played.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        Console.WriteLine("Please try again.");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("==============================");
                Console.WriteLine("         GAME ENDED");
                Console.WriteLine("==============================");
                Console.WriteLine();

                Console.Write("Play again? (y/n): ");
                string? yOrN = Console.ReadLine();

                while (!string.Equals(yOrN, "y",
                           StringComparison.OrdinalIgnoreCase)
                       && !string.Equals(yOrN, "n",
                           StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Invalid input. Enter 'y' or 'n': ");
                    yOrN = Console.ReadLine();
                }

                if (string.Equals(yOrN, "n",
                    StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine();
                    Console.WriteLine("Thanks for playing Shiritori!");
                    break;
                }
                else
                {
                    game.NewGame();

                    Console.WriteLine();
                    Console.WriteLine("==============================");
                    Console.WriteLine("          NEW GAME");
                    Console.WriteLine("==============================");
                }
            }
        }


        public static void PrintWelcome()
        {
                Console.WriteLine("=== ENGLISH SHIRITORI ===");
Console.WriteLine("Play a word that starts with the last letter of the previous word.");
Console.WriteLine("Gemini will check your word and may correct spelling mistakes.");
Console.WriteLine("Words cannot be repeated.");
Console.WriteLine("If Gemini cannot find a valid reply, you win!");
Console.WriteLine();
        }

        public static void RunFakeServiceChecks() 
        {
            Console.WriteLine("Check 1");
            Check1();
            Console.WriteLine();
            Console.WriteLine("Check 2");
            Check2();
            Console.WriteLine();
            Console.WriteLine("Check 3");
            Check3();
            Console.WriteLine();
            Console.WriteLine("Check 4");
            Check4();
            Console.WriteLine();
            Console.WriteLine("Check 5");
            Check5();
        }

        public static async void Check1() 
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

        public static async void Check2()
        {
            string answer = """
    {"status":"notAWord","correctedWord":null,"replyWord":null,"explanation":null}
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

        public static async void Check3()
        {
            string answer = """
    {"status":"noWordLeft","correctedWord":"apple","replyWord":null,"explanation":null}
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

        public static async void Check4()
        {
            List<string> answers = new List<string>();

            answers.Add("""
    {"status":"ok","correctedWord":"apple","replyWord":"banana","explanation":"A fruit"}
    """);

            answers.Add("""
    {"status":"ok","correctedWord":"apple","replyWord":"elephant","explanation":"An animal"}
    """);

            FakeGeminiService fake = new FakeGeminiService(answers);

            Shiritori game = new Shiritori(fake);

            MoveResult result = await game.PlayAsync("apple");

            Console.WriteLine($"Outcome: {result.Outcome}");
            Console.WriteLine($"History count: {game.History.Count}");
            Console.WriteLine($"State: {game.State}");
        }

        public static async void Check5()
        {
            List<string> answers = new List<string>();

            answers.Add("""
    {"status":"ok","correctedWord":"apple","replyWord":"banana","explanation":"A fruit"}
    """);

            answers.Add("""
    {"status":"ok","correctedWord":"apple","replyWord":"cat","explanation":"An animal"}
    """);

            answers.Add("""
    {"status":"ok","correctedWord":"apple","replyWord":"dog","explanation":"An animal"}
    """);

            FakeGeminiService fake = new FakeGeminiService(answers);

            Shiritori game = new Shiritori(fake);

            MoveResult result = await game.PlayAsync("apple");

            Console.WriteLine($"Player word: {result.PlayerTurn?.Word}");
            Console.WriteLine($"Outcome: {result.Outcome}");
            Console.WriteLine($"History count: {game.History.Count}");
            Console.WriteLine($"State: {game.State}");
        }
    }
}