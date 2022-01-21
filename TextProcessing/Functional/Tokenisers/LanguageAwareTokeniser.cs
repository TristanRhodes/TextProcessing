using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessing.Functional.Tokenisers
{
    public class LanguageAwareTokeniser
    {
        Regex _splitPattern;
        Dictionary<Language, TokenParser[]> _languageTokenisers = new Dictionary<Language, TokenParser[]>();

        public LanguageAwareTokeniser(string splitPattern)
        {
            _splitPattern = new Regex(splitPattern);
        }

        public void AddLanguage(Language language, params TokenParser[] parsers)
        {
            _languageTokenisers.Add(language, parsers);
        }

        public IEnumerable<Token> Tokenise(string inputString, Language language)
        {
            if (!_languageTokenisers.ContainsKey(language))
                throw new ApplicationException("Language not loaded: " + language);

            var parts = _splitPattern.Split(inputString);
            foreach (var part in parts)
            {
                var match = _languageTokenisers[language]
                    .Select(processor => processor(part))
                    .Where(t => t.Successful)
                    .FirstOrDefault();

                yield return match == null ?
                    Token.Create(part) :
                    match.Token;
            }
        }

    }

    public enum Language
    {
        English,
        German
    }
}
