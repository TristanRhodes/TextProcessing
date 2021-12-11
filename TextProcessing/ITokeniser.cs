using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    public interface ITokeniser
    {
        bool IsMatch(string token);

        Token Tokenise(string token);
    }
}
