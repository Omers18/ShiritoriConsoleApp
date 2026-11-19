using Mscc.GenerativeAI.Types;
using ShiritoriConsoleApp.Gemini;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace ShiritoriConsoleApp.Models
{
    internal class Shiritori
    {
        public Shiritori(IGeminiService service)
        {
            this.service = service;
            History = new TurnHistory();
            Protocol = new GeminiProtocol();
            State = GameState.InProgress;
        }

        public TurnHistory History { get;}
        public GameState State { get; private set; }
        public char? RequiredLetter 
        {
            get 
            {
                if (History.Count == 0) return null;
                string word = History.LastTurn.Word;
                return word[word.Length - 1];
            }
        }

        public GeminiProtocol Protocol { get;}
        private IGeminiService service;


        public bool IsLowercaseEnglish(string? word) 
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                if (c < 'a' || c > 'z')
                {
                    return false;
                }
            }
            return true;
        }

        private bool CorrectedWordCheck(string? correctedWord)
        {
            if (correctedWord == null)
            {
                return false;
            }

            string normalizedC = correctedWord.Trim().ToLowerInvariant();

            // Must contain only English letters and not be empty
            if (!IsLowercaseEnglish(normalizedC))
            {
                return false;
            }

            // The corrected word cannot have been played before
            if (History.Contains(normalizedC))
            {
                return false;
            }

            // Must start with the required letter, if there is one
            if (RequiredLetter != null && normalizedC[0] != RequiredLetter)
            {
                return false;
            }

            return true;
        }

        private bool GeminiWordCheck(string? correctedWord, string? replyWord)
        {
            if (correctedWord == null || replyWord == null)
            {
                return false;
            }

            string normalizedC = correctedWord.Trim().ToLowerInvariant();
            string normalizedR = replyWord.Trim().ToLowerInvariant();

            // Both words must contain English letters and not be empty
            if (!IsLowercaseEnglish(normalizedC) ||
                !IsLowercaseEnglish(normalizedR))
            {
                return false;
            }

            // Gemini cannot repeat the player's corrected word
            if (normalizedC == normalizedR)
            {
                return false;
            }

            // Gemini cannot use a word that was already played
            if (History.Contains(normalizedR))
            {
                return false;
            }

            // Gemini's word must start with the last letter
            // of the player's corrected word
            if (normalizedR[0] != normalizedC[normalizedC.Length - 1])
            {
                return false;
            }

            return true;
        }

        public async Task<MoveResult> PlayAsync(string playerWord)
        {
            if (State != GameState.InProgress)
            {
                throw new InvalidOperationException("The game has already ended");
            }

            MoveOutcome outcome;

            string normalizedWord = playerWord.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalizedWord) || !IsLowercaseEnglish(normalizedWord))
            {
                outcome = MoveOutcome.NotWellFormed;
                return new MoveResult(outcome, null, null);
            }

            else if(RequiredLetter != null && normalizedWord[0] != RequiredLetter) 
            {
                outcome = MoveOutcome.WrongLetter;
                return new MoveResult(outcome, null, null);
            }

            else if (History.Contains(normalizedWord))
            {
                outcome = MoveOutcome.AlreadyUsed;
                return new MoveResult(outcome, null, null);
            }

            string prompt = Protocol.BuildPrompt(History.UsedWords(), normalizedWord);

            string? acceptedWord = null;    
            for (int i = 0; i < 3; i++)
            {
                string responseText = await service.AskAsync(prompt);
                GeminiReply reply = Protocol.ParseReply(responseText);

                if (reply.Status == ReplyStatus.NotAWord)
                {
                    if (acceptedWord != null)
                    {
                        throw new FormatException(
                            "Gemini changed its decision about the player's word");
                    }

                    State = GameState.GeminiWon;
                    return new MoveResult(MoveOutcome.NotAWord, null, null);
                }

                else if (reply.Status == ReplyStatus.NoWordLeft)
                {
                    if (!CorrectedWordCheck(reply.CorrectedWord))
                    {
                        throw new FormatException("Invalid corrected word");
                    }

                    string correctedWord = reply.CorrectedWord!.Trim().ToLowerInvariant();

                    if (acceptedWord != null && acceptedWord != correctedWord)
                    {
                        throw new FormatException(
                            "Corrected word changed between attempts"
                        );
                    }

                    Turn playerTurn = new Turn(History.Count + 1, Player.Human,
                        correctedWord, playerWord, null);

                    History.Add(playerTurn);
                    State = GameState.PlayerWon;

                    return new MoveResult(MoveOutcome.PlayerWins, playerTurn, null);
                }

                else
                {
                    // 1. Check the player's corrected word
                    if (!CorrectedWordCheck(reply.CorrectedWord))
                    {
                        throw new FormatException("Invalid corrected word");
                    }

                    string correctedWord = reply.CorrectedWord!.Trim().ToLowerInvariant();

                  
                    // 2. Remember the accepted word
                    if (acceptedWord == null)
                    {
                        acceptedWord = correctedWord;
                    }

                    else if (acceptedWord != correctedWord)
                    {
                        throw new FormatException("Corrected word changed between attempts");
                    }

                    // 3. Check Gemini's reply word
                    if (!GeminiWordCheck(correctedWord, reply.ReplyWord))
                    {
                        continue;
                    }

                    string replyWord = reply.ReplyWord!.Trim().ToLowerInvariant();

                    // Create Turns and return Accepted
                    Turn humanTurn = new Turn(History.Count + 1, Player.Human,
                        correctedWord, playerWord, null);

                    Turn geminiTurn = new Turn(History.Count + 2, Player.Gemini,
                        replyWord, replyWord, reply.Explanation);

                    History.Add(humanTurn); History.Add(geminiTurn);
                    outcome = MoveOutcome.Accepted;
                    return new MoveResult(outcome, humanTurn, geminiTurn);
                }
            }

            if (acceptedWord == null)
            {
                throw new InvalidOperationException("No accepted player word");
            }

            Turn playerWinningTurn = new Turn(History.Count + 1, Player.Human,
                acceptedWord, playerWord,null);

            History.Add(playerWinningTurn);
            State = GameState.PlayerWon;
            outcome = MoveOutcome.PlayerWins;

            return new MoveResult(outcome, playerWinningTurn, null);
        }
    }
}
