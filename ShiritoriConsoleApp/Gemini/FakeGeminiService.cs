using System;
using System.Collections.Generic;
using System.Text;

namespace ShiritoriConsoleApp.Gemini
{
    internal class FakeGeminiService : IGeminiService
    {
        private List<string> _answers;
        private int _index;

        public FakeGeminiService(List<string> answers)
        {
            _answers = answers;
            _index = 0;
        }

        public Task<string> AskAsync(string prompt)
        {
            if (_index >= _answers.Count)
            {
                throw new InvalidOperationException("No more fake Gemini responses");
            }
            Task<string> result = Task.FromResult(_answers[_index]);
            _index++;
            return result;
        }
    }
}
