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

            // A Function that takes a function and returns a function that adds 1 to 
            // the result.
            Func<Func<int>, Func<int>> add = (Func<int> baseValue) =>
                () => baseValue() + 1;

            // A function returning a baseValue() inside an Add(), inside an Add()
            Func<int> result =
                add(
                    add(
                        baseValue));

            // Because we ran add() twice, we get a value of 2.
            result()
                .Should().Be(2);
        }

        [Fact]
        public void MonadNestingTest()
        {
            Func<int> baseValue = () => 0;

            // A Function that takes value and a function and returns a function that 
            // adds the value to the result of the function.
            Func<int, Func<int>, Func<int>> add =
                (int value, Func<int> baseValue) =>
                    () => baseValue() + value;

            // A function returning a baseValue, inside an add(1), inside an add(2), 
            // inside an add(3)
            Func<int> result =
                add(3,
                    add(2,
                        add(1,
                            baseValue)));

            result()
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
        public void MonadNestingFunctorTest()
        {
            Func<int> baseValue = () => 0;

            // A Function that takes value and a function and returns a function that 
            // adds the value to the result of the function.
            Func<int, Func<int>, Func<int>> add =
                (int value, Func<int> baseValue) =>
                    () => baseValue() + value;

            // Function that takes a Func<int> and returns a Func<string> that runs the Func<int>,
            // turns the result into a string and prefixes it with the £ symbol
            Func<Func<int>, Func<string>> asGBPString =
                (Func<int> baseValue) =>
                    () => $"£" + baseValue();

            // A function returning a baseValue, inside an add(1), inside an add(2), 
            // inside an add(3), inside an asString()
            Func<string> price =
                asGBPString(
                    add(3,
                        add(2,
                            add(1,
                                baseValue))));

            price()
                .Should().Be("£6");
        }

        [Fact]
        public void MonadChainFluentTest()
        {
            Func<string> price =
                Beginning
                    .With(0)
                    .Add(1)
                    .Add(2)
                    .Add(3)
                    .AsGBPString();

            price()
                .Should().Be("£6");
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

        public static Func<string> AsGBPString(this Func<int> baseValue) =>
            () => $"£" + baseValue();
    }
}
