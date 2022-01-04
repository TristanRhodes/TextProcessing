using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.OO.Tokenisers
{
    public interface ITokenProcessor
    {
        bool IsMatch(string token);
        Token Tokenise(string token);
    }
}
