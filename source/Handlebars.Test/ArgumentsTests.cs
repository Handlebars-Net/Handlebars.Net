using System.Linq;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class ArgumentsTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        public void ArgumentsCtor(int count)
        {
            var values = new object[count];
            CreateValues(values);
            var arguments = CreateArguments(values);

            for (var index = 0; index < values.Length; index++)
            {
                Assert.Equal(values[index], arguments[index]);
            }
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        public void ArgumentsEnumerator(int count)
        {
            var values = new object[count];
            CreateValues(values);
            var arguments = CreateArguments(values);

            var valuesEnumerator = values.GetEnumerator();
            var argumentsEnumerator = arguments.GetEnumerator();

            for (int index = 0; index < values.Length; index++)
            {
                Assert.Equal(valuesEnumerator.MoveNext(), argumentsEnumerator.MoveNext());
                Assert.Equal(valuesEnumerator.Current, argumentsEnumerator.Current);
            }
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        public void ArgumentsEnumerable(int count)
        {
            var values = new object[count];
            CreateValues(values);
            var arguments = CreateArguments(values);

            using var valuesEnumerator = values.AsEnumerable().GetEnumerator();
            using var argumentsEnumerator = arguments.GetEnumerator();

            for (int index = 0; index < values.Length; index++)
            {
                Assert.Equal(valuesEnumerator.MoveNext(), argumentsEnumerator.MoveNext());
                Assert.Equal(valuesEnumerator.Current, argumentsEnumerator.Current);
            }
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        public void ArgumentsAdd(int count)
        {
            var values = new object[count];
            CreateValues(values);
            var arguments = CreateArguments(values);

            var value = new object();
            var newArguments = arguments.Add(value);
            
            Assert.Equal(arguments.Length + 1, newArguments.Length);
            Assert.Equal(value, newArguments[newArguments.Length - 1]);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        public void ArgumentsEquals(int count)
        {
            var values = new object[count];
            CreateValues(values);
            var arguments1 = CreateArguments(values);
            var arguments2 = CreateArguments(values);
            
            Assert.Equal(arguments1, arguments2);
        }

        private static void CreateValues(object[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = new object();
            }
        }
        
        private static Arguments CreateArguments(object[] values)
        {
            int count = values.Length;
            var constructor =
                typeof(Arguments)
                    .GetConstructors()
                    .SingleOrDefault(o =>
                    {
                        var parameterInfos = o.GetParameters();
                        return parameterInfos.Length == count && parameterInfos[0].ParameterType != typeof(object[]);
                    })
                ??
                typeof(Arguments)
                    .GetConstructors()
                    .Single(o =>
                    {
                        var parameterInfos = o.GetParameters();
                        return parameterInfos.Length == 1 && parameterInfos[0].ParameterType == typeof(object[]);
                    });

            Arguments arguments;
            if (values.Length <= 6)
            {
                arguments = (Arguments) constructor.Invoke(values);
            }
            else
            {
                arguments = (Arguments) constructor.Invoke(new object[] {values});
            }

            return arguments;
        }
    }
}