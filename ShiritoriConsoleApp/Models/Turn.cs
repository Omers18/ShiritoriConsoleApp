using System;
using System.Collections.Generic;
using System.Text;

namespace ShiritoriConsoleApp.Models
{
    internal class Turn
    {
        public Turn(int number, Player player, string word, string originalWord, string? explanation)
        {
            Number = number;
            Player = player;
            Word = word;
            OriginalWord = originalWord;
            Explanation = explanation;  
        }

        public int Number { get; }
        public Player Player { get; }
        public string Word { get; }
        public string OriginalWord { get;}
        public string? Explanation { get; }
        public bool WasCorrected { get => !string.Equals(Word, OriginalWord, StringComparison.OrdinalIgnoreCase); }
        public char LastLetter {get => Word[Word.Length - 1]; }


    }
}
