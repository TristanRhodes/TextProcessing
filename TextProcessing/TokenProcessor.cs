using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextProcessing
{
    public class TokenProcessor
    {
        Regex _splitPattern;
        IList<ITokeniser> _tokenisers;

        public TokenProcessor(string splitPattern, IEnumerable<ITokeniser> tokenisers)
        {
            _splitPattern = new Regex(splitPattern);
            _tokenisers = tokenisers.ToList();
        }

        public TokenProcessor(string splitPattern, params ITokeniser[] tokenisers)
        {
            _splitPattern = new Regex(splitPattern);
            _tokenisers = tokenisers.ToList();
        }

        public IEnumerable<Token> Tokenise(string inputString)
        {
            var parts = _splitPattern.Split(inputString);
            foreach(var part in parts)
            {
                var matches = _tokenisers
                    .Where(t => t.IsMatch(part))
                    .Select(t => t.Tokenise(part));

                var count = matches.Count();
                if (count > 1)
                    throw new ApplicationException($"Ambiguous match on token '{part}'. Collisions: '{string.Join("', '", matches)}'");

                yield return count == 0 ?
                    Token.Create(part) :
                    matches.Single();
            }
        }
    }
}
