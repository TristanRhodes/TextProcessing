using System;
using System.Text.RegularExpressions;
using TextProcessing.Model;

namespace TextProcessing.Tokenisers
{
    public class FlagTokenProcessor : ITokenProcessor
    {
        Regex regex = new Regex(@"^(?<pickup>[Pp]ickup)|(?<dropoff>[Dd]ropoff)|(?<open>[Oo]pen)|(?<tours>[Tt]ours)|(?<events>[Ee]vents)$");

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

            if (match.Groups["open"].Success)
                return Token.Create(token, new OpenFlag());

            if (match.Groups["tours"].Success)
                return Token.Create(token, new ToursFlag());

            if (match.Groups["events"].Success)
                return Token.Create(token, new EventsFlag());

            throw new ApplicationException("No Match: " + token);
        }
    }
}
