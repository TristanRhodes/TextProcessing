using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.OO.Tokenisers;

namespace TextProcessing.Tests
{
    public static class Extensions
    {
        public static bool IsMatch(this ITokenParser parser, string text)
        {
            return parser
                .Tokenise(text)
                .Successful;
        }
    }
}
