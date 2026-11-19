using System;
using System.Collections.Generic;
using System.Text;

namespace ShiritoriConsoleApp.Gemini
{
    internal enum ReplyStatus
    {
        Ok,
        NotAWord,
        NoWordLeft
    }

    internal class GeminiReply
    {
        public GeminiReply(ReplyStatus status, string? correctedWord, string? replyWord, string? explanation)
        {
            Status = status;
            CorrectedWord = correctedWord;
            ReplyWord = replyWord;
            Explanation = explanation;
        }

        public ReplyStatus Status { get; }
        public string? CorrectedWord { get; }
        public string? ReplyWord { get; }
        public string? Explanation { get; }
    }
}
