using System.Text;
using System.Threading.Tasks;

namespace TextProcessing.OOTokenisers
{
    public interface ITokenProcessor
    {
        bool IsMatch(string token);
        Token Tokenise(string token);
    }
}
