using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextProcessing.OO.Tokenisers
{
    public interface ITokenParser
    {
        TokenisationResult Tokenise(string token);
    }

    public record TokenisationResult(Token Token, bool Success);

    public class Tokeniser
    {
        Regex _splitPattern;
        IList<ITokenParser> _tokenisers;

        public Tokeniser(string splitPattern, params ITokenParser[] tokenisers)
        {
            _splitPattern = new Regex(splitPattern);
            _tokenisers = tokenisers.ToList();
        }

        public IEnumerable<Token> Tokenise(string inputString)
        {
            var parts = _splitPattern.Split(inputString);
            foreach(var part in parts)
            {
                var match = _tokenisers
                    .Select(t => t.Tokenise(part))
                    .Where(t => t.Success)
                    .FirstOrDefault();

                yield return match == null ?
                    Token.Create(part) :
                    match.Token;
            }
        }
    }
}
