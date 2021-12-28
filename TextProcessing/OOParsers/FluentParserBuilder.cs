using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing.Model;

namespace TextProcessing.OOParsers
{
    public class ParserBuilder<T>
    {
        Parser<T> _core;

        public ParserBuilder()
        {
            _core = new Is<T>();
        }

        public ParserBuilder(Parser<T> core)
        {
            _core = core;
        }

        public Func<ParserBuilder<T>> Single<TResult>(Func<ParserBuilder<T>, TResult, Func<ParserBuilder<T>>> p)
        {
            TResult result = default(TResult);

            var core = new Is<TResult>();
            var then = new Then<T, TResult>(_core, (t) => { return null; });

            return () => this;
        }

        public Func<ParserBuilder<T>> Then<TResult>(Func<ParserBuilder<T>, TResult, Func<ParserBuilder<T>>> p)
        {
            throw new NotImplementedException();
        }

        public Func<ParserBuilder<T>> Or<TResult>(Func<ParserBuilder<T>, TResult, Func<ParserBuilder<T>>> p)
        {
            throw new NotImplementedException();
        }

        public Func<ParserBuilder<T>> Select<TResult>(TResult result)
        {
            throw new NotImplementedException();
            //return () => new ParserBuilder<TResult>(new Select<TResult>(result));
        }

        public Parser<T> Build()
        {
            throw new NotImplementedException();
        }
    }
}
