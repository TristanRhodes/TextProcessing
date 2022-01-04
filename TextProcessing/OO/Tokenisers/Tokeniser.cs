using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextProcessing.OO.Tokenisers
{
    public class Tokeniser
    {
        Regex _splitPattern;
        IList<ITokenProcessor> _tokenisers;

        public Tokeniser(string splitPattern, params ITokenProcessor[] tokenisers)
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
                    .Where(t => t.IsMatch(part))
                    .Select(t => t.Tokenise(part))
                    .FirstOrDefault();

                yield return match == null ?
                    Token.Create(part) :
                    match;
            }
        }
    }
}
