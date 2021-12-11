using System;
using System.Collections.Generic;
using System.Linq;

namespace TextProcessing
{
    public class TokenProcessor
    {
        IList<ITokeniser> _tokenisers;

        public TokenProcessor(IEnumerable<ITokeniser> tokenisers)
        {
            _tokenisers = tokenisers.ToList();
        }

        public TokenProcessor(params ITokeniser[] tokenisers)
        {
            _tokenisers = tokenisers.ToList();
        }

        public Token[] Tokenise(string[] tokenStrings)
        {
            var tokens = new Token[tokenStrings.Length];
            for(int i=0; i<tokenStrings.Length;i++)
            {
                var str = tokenStrings[i];
                var matches = _tokenisers
                    .Where(t => t.IsMatch(str));

                if (matches.Count() > 1)
                    throw new ApplicationException($"Ambiguous match on token '{str}'. Collisions: '{string.Join("', '", matches)}'");

                tokens[i] = matches
                    .Single()
                    .Tokenise(str);
            }

            return tokens;
        }
    }
}
