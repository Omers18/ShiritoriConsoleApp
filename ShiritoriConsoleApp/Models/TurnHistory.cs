using System;
using System.Collections.Generic;
using System.Text;

namespace ShiritoriConsoleApp.Models
{
    internal class TurnHistory
    {
        private List<Turn> _turns = new List<Turn>();
        public int Count { get => _turns.Count; }
        public void Add(Turn turn)
        {
            _turns.Add(turn);
        }
        public bool Contains(string word)
        {
            for(int i =0; i < _turns.Count; i++) 
            {
                if (string.Equals(word.Trim(), _turns[i].Word.Trim(), StringComparison.OrdinalIgnoreCase ))
                {
                    return true;
                }
            }
            return false;
        }

        public Turn? LastTurn 
        { 
            get
            {
                if (_turns.Count == 0)
                {
                    return null;
                }
                int lastIndex = _turns.Count - 1;
                return _turns[lastIndex];
            }  
        }
        public List<Turn> Turns { get => _turns; }
        public List<string> UsedWords() 
        {
            List<string> usedWords = new List<string>();
            for (int i = 0; i < _turns.Count; i++)
            {
                string word = _turns[i].Word;
                usedWords.Add(word);
            }
            return usedWords;
        }

        public void Clear()
        {
            _turns.Clear();
        }
    }
}
