using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TextProcessing.Tests
{
    public class MonadTests
    {
        [Fact]
        public void BasicNestedFunctionTest()
        {
            // Function that returns a starting value of 0
            Func<int> baseValue = () => 0;

            // Function that returns a function that adds 1
            // to the result of the basevalue function.
            Func<int> add = () =>
                baseValue() + 1;

            // Run the add function, returning the result of basevalue, + 1
            add()
                .Should().Be(1);
        }

        [Fact]
        public void NestedFunctionTest()
        {
            Func<int> baseValue = () => 0;

            Func<Func<int>, Func<int>> add = (Func<int> baseValue) =>
                () => baseValue() + 1;

            Func<int> result = 
                add(
                    add(
                        baseValue));

            result()
                .Should().Be(2);
        }

        [Fact]
        public void MonadNestingTest()
        {
            Func<int> baseValue = () => 0;

            Func<int, Func<int>, Func<int>> add =
                (int value, Func<int> baseValue) =>
                    () => baseValue() + value;

            Func<int> total =
                add(3,
                    add(2,
                        add(1,
                            baseValue)));

            total()
                .Should().Be(6);
        }

        /// <summary>
        /// .Net 6.0 new feature - inferred delegate types.
        /// </summary>
        [Fact]
        public void MonadNestingVarTest()
        {
            var baseValue = () => 0;

            var add = (int value, Func<int> baseValue) =>
                    () => baseValue() + value;

            var total =
                add(3,
                    add(2,
                        add(1,
                            baseValue)));

            total()
                .Should().Be(6);
        }

        [Fact]
        public void MonadChainFluentTest()
        {
            Func<int> total =
                Beginning
                    .With(0)
                    .Add(1)
                    .Add(2)
                    .Add(3);

            total().Should().Be(6);
        }
    }

    public static class Beginning
    {
        public static Func<int> With(int value) =>
            () => value;
    }

    public static class MonadExtensions
    {
        public static Func<int> Add(this Func<int> baseValue, int value) =>
            () => baseValue() + value;
    }
}
