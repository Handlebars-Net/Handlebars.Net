using HandlebarsDotNet;
using HandlebarsDotNet.Compiler.Lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Handlebars.Test.Compiler.Lexer
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("{{a}}")]
        [InlineData("{{a}}}")]
        [InlineData("a")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("(a)")]
        public void TokensOriginalValueTest(string input)
        {
            // Arrange
            var tokenizer = new Tokenizer(new HandlebarsConfiguration());

            using (var sr = new StringReader(input))
            {
                // Act
                var tokens = tokenizer.Tokenize(sr).ToList();

                // Assert
                var tokenString = TokensToString(tokens);

                Assert.Equal(tokenString, input);
            }
        }

        private static object TokensToString(IEnumerable<Token> tokens)
        {
            return string.Join("", tokens.Select(t => t.OriginalValue));
        }
    }
}
