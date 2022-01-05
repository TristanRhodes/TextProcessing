using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessing.Monad.Tokenisers
{
    public delegate TokenisationResult TokenParser(string token);

    public record TokenisationResult(Token Token, bool Success);

    public class Tokeniser
    {
        Regex _splitPattern;
        IList<TokenParser> _tokenisers;

        public Tokeniser(string splitPattern, params TokenParser[] tokenisers)
        {
            _splitPattern = new Regex(splitPattern);
            _tokenisers = tokenisers.ToList();
        }

        public IEnumerable<Token> Tokenise(string inputString)
        {
            var parts = _splitPattern.Split(inputString);
            foreach (var part in parts)
            {
                var match = _tokenisers
                    .Select(processor => processor(part))
                    .Where(t => t.Success)
                    .FirstOrDefault();

                yield return match == null ?
                    Token.Create(part) :
                    match.Token;
            }
        }

        public static TokenParser FromRegex(string pattern, Func<Match, TokenisationResult> resolver)
        {
            var regex = new Regex(pattern);
            return (string token) =>
            {
                var match = regex.Match(token);

                if (!match.Success)
                    return Token.Fail(token);

                return resolver(match);
            };
        }

        public static TokenParser FromChar(char c, Func<char, TokenisationResult> resolver)
        {
            return (string token) =>
            {
                return (token.Length == 1 && token[0] == c) ?
                    resolver(token[0]) :
                    Token.Fail(token);
            };
        }
    }
}
