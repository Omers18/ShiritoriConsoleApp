using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ShiritoriConsoleApp.Gemini
{
    internal class GeminiProtocol
    {
        public string BuildPrompt(IReadOnlyList<string> usedWords, string playerWord)
        {
            string instructions = @"We are playing English Shiritori. Each word must start with the last letter of
the previous word. No word may be used twice.

I will send a JSON object with these keys:
  ""playerWord"": the player's word, possibly misspelled
  ""usedWords"": all earlier words in the game

Fix the spelling of playerWord, but do not replace it with a different word.
Then reply with a word that starts with the last letter of the corrected word,
is not in usedWords, and is not the corrected word.
Use only common dictionary words, in lowercase letters, one word.

Reply with ONLY a JSON object, with no extra text, using these keys:
  ""status"": ""ok"", ""notAWord"" or ""noWordLeft""
  ""correctedWord"": the player's word after fixing the spelling
  ""replyWord"": your word
  ""explanation"": a short dictionary definition of replyWord

Use ""notAWord"" if playerWord is not an English word you can recognize even
after fixing the spelling. Use ""noWordLeft"" if you cannot find a valid reply.
In both cases, use null for every key that does not apply.";

            var request = new
            {
                playerWord = playerWord,
                usedWords = usedWords
            };

            string json = JsonSerializer.Serialize(request);

            string prompt = $"{instructions}\n\n{json}";
            return prompt;
        }

        public GeminiReply ParseReply(string text)
        {
            ReplyStatus status;
            string? correctedWord;
            string? replyWord;
            string? explanation;

            try
            {
                using (JsonDocument document = JsonDocument.Parse(text))
                {
                    string? statusText = document.RootElement.GetProperty("status").GetString();
                    correctedWord = document.RootElement.GetProperty("correctedWord").GetString();
                    replyWord = document.RootElement.GetProperty("replyWord").GetString();
                    explanation = document.RootElement.GetProperty("explanation").GetString();

                    if (statusText == "ok")
                    {
                        if (string.IsNullOrWhiteSpace(correctedWord) ||
                            string.IsNullOrWhiteSpace(replyWord) ||
                            string.IsNullOrWhiteSpace(explanation))
                        {
                            throw new FormatException("Missing data in an OK reply");
                        }

                        status = ReplyStatus.Ok;
                    }
                    else if (statusText == "notAWord")
                    {
                        if (replyWord != null ||
                            correctedWord != null ||
                            explanation != null)
                        {
                            throw new FormatException("Fields must be null in a notAWord reply");
                        }

                        status = ReplyStatus.NotAWord;
                    }
                    else if (statusText == "noWordLeft")
                    {
                        if (string.IsNullOrWhiteSpace(correctedWord) ||
                            replyWord != null ||
                            explanation != null)
                        {
                            throw new FormatException("Invalid fields in a noWordLeft reply");
                        }

                        status = ReplyStatus.NoWordLeft;
                    }
                    else
                    {
                        throw new FormatException("Invalid status");
                    }
                }

                GeminiReply reply = new GeminiReply(
                    status, correctedWord, replyWord, explanation);

                return reply;
            }

            catch (JsonException ex)
            {
                throw new FormatException("Invalid JSON reply", ex);
            }
            catch (KeyNotFoundException ex)
            {
                throw new FormatException("Missing JSON property", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new FormatException("Content must be a string or null", ex);
            }
        }
    }
}