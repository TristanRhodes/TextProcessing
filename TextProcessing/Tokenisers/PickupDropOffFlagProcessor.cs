using System;
using System.Text.RegularExpressions;
using TextProcessing.Model;

namespace TextProcessing.Tokenisers
{
    public class PickupDropOffFlagProcessor : ITokenProcessor
    {
        Regex regex = new Regex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            var match = regex.Match(token);
            
            if (match.Groups["pickup"].Success)
                return Token.Create(token, new PickupFlag());

            if (match.Groups["dropoff"].Success)
                return Token.Create(token, new DropoffFlag());

            throw new ApplicationException("No Match: " + token);
        }
    }
}
