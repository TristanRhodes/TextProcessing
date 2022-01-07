using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextProcessing.OO.Tokenisers
{
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
            foreach (var part in parts)
            {
                var match = _tokenisers
                    .Select(t => t.Tokenise(part))
                    .Where(t => t.Successful)
                    .FirstOrDefault();

                yield return match == null ?
                    Token.Create(part) :
                    match.Token;
            }
        }
    }

    public interface ITokenParser
    {
        TokenisationResult Tokenise(string token);
    }

    public class TokenisationResult
    {
        Token _token;

        public TokenisationResult() 
        {
            Successful = false;
        }
        public TokenisationResult(Token token)
        {
            _token = token;
            Successful = true;
        }

        public Token Token => Successful ? _token : throw new ApplicationException("Not Successful");

        public bool Successful  { get; init; }

        public static TokenisationResult Fail() =>
            new TokenisationResult();

        public static TokenisationResult Success(object value) =>
            new TokenisationResult(Token.Create(value));
    }


}
