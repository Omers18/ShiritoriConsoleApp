using System;
using System.Collections.Generic;
using System.Text;

namespace ShiritoriConsoleApp.Models
{
    internal enum GameState 
    {
        InProgress, PlayerWon, GeminiWon
    }

    internal enum MoveOutcome
    {
        Accepted, NotWellFormed, NotAWord, 
        WrongLetter, AlreadyUsed, PlayerWins
    }

    internal class MoveResult
    {
        public MoveResult(MoveOutcome outcome, Turn? playerTurn, Turn? geminiTurn)
        {
            Outcome = outcome;
            PlayerTurn = playerTurn;
            GeminiTurn = geminiTurn;
        }

        public MoveOutcome Outcome { get;}
        public Turn? PlayerTurn { get; }
        public Turn? GeminiTurn { get; }
    }
}
