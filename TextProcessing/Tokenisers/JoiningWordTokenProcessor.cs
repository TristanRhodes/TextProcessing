using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.Tokenisers
{
    public class JoiningWordTokenProcessor : ITokenProcessor
    {
        Regex regex = new Regex(@"^[Tt]o$");

        public bool IsMatch(string token)
        {
            return regex.IsMatch(token);
        }

        public Token Tokenise(string token)
        {
            if (!IsMatch(token))
                throw new ApplicationException("Bad Match: " + token);

            return Token.Create(token, new JoiningWord());
        }
    }
}
