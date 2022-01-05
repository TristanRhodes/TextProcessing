using NodaTime;
using System;
using TextProcessing.Model;
using TextProcessing.OO.Tokenisers;

namespace TextProcessing.OO.SimpleParsers
{
    /// <summary>
    /// Demo parser for most naive approach
    /// </summary>
    public class SimpleDayTimeParser : Parser<DayTime>
    {
        public override ParseResult<DayTime> Parse(Token[] tokens)
        {
            if (tokens.Length != 2)
                return ParseResult<DayTime>.Failure();
            if (!tokens[0].Is<DayOfWeek>())
                return ParseResult<DayTime>.Failure();
            if (!tokens[1].Is<LocalTime>())
                return ParseResult<DayTime>.Failure();

            var result = new DayTime(
                tokens[0].As<DayOfWeek>(),
                tokens[1].As<LocalTime>());

            return ParseResult<DayTime>
                .Successful(result);
        }
    }
}
