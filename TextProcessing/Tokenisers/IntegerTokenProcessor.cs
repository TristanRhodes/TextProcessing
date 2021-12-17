using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.Tokenisers
{
    public class IntegerTokenProcessor : ITokenProcessor
    {
        public bool IsMatch(string token)
        {
            return int.TryParse(token, out int _);
        }

        public Token Tokenise(string token)
        {
            if (!IsMatch(token))
                throw new ApplicationException("Bad Match: " + token);

            return Token.Create(token, int.Parse(token));
        }
    }
}
