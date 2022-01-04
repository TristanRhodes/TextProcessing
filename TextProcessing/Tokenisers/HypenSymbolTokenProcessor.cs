using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.Tokenisers
{
    public class HypenSymbolTokenProcessor : ITokenProcessor
    {
        public bool IsMatch(string token)
        {
            return token == "-";
        }

        public Token Tokenise(string token)
        {
            if (!IsMatch(token))
                throw new ApplicationException("Bad Match: " + token);

            return Token.Create(token, new HypenSymbol());
        }
    }
}
