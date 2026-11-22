using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Features;
using Xunit;

namespace HandlebarsDotNet.Test
{
    /// <summary>
    /// Covers rendering when expression trees are interpreted instead of compiled to IL.
    /// That is what happens on runtimes without dynamic code generation: on Blazor WebAssembly,
    /// iOS/tvOS/Mac Catalyst and NativeAOT, <see cref="LambdaExpression.Compile()"/> always
    /// falls back to the System.Linq.Expressions interpreter.
    /// </summary>
    // https://github.com/Handlebars-Net/Handlebars.Net/issues/345
    public class ExpressionInterpretationTests
    {
        public static IEnumerable<object[]> Templates()
        {
            yield return new object[] { "Hello {{name}}!", new { name = "World" }, "Hello World!" };
            yield return new object[] { "{{person.address.city}}", new { person = new { address = new { city = "Oslo" } } }, "Oslo" };
            yield return new object[] { "{{text}} {{{text}}}", new { text = "<b>" }, "&lt;b&gt; <b>" };
            yield return new object[] { "{{#if flag}}yes{{else}}no{{/if}}", new { flag = false }, "no" };
            yield return new object[] { "{{#unless flag}}yes{{/unless}}", new { flag = false }, "yes" };
            yield return new object[] { "{{#each items}}{{@index}}:{{this}};{{/each}}", new { items = new[] { "a", "b" } }, "0:a;1:b;" };
            yield return new object[] { "{{#each items}}{{@key}}={{this}};{{/each}}", new { items = new Dictionary<string, int> { ["x"] = 1, ["y"] = 2 } }, "x=1;y=2;" };
            yield return new object[] { "{{#each items}}{{name}}{{else}}empty{{/each}}", new { items = new object[0] }, "empty" };
            yield return new object[] { "{{#with person}}{{name}} {{@root.suffix}}{{/with}}", new { person = new { name = "Ann" }, suffix = "!" }, "Ann !" };
            yield return new object[] { "{{lookup map key}}", new { map = new Dictionary<string, string> { ["k"] = "v" }, key = "k" }, "v" };
            yield return new object[] { "{{shout (concat first last)}}", new { first = "a", last = "b" }, "AB" };
            yield return new object[] { "{{#wrap}}inner {{name}}{{else}}inverse{{/wrap}}", new { name = "x" }, "[inner x]" };
            yield return new object[] { "{{> greeting}}", new { name = "Bob" }, "Hi Bob" };
            yield return new object[] { "{{> greeting name=\"Eve\"}}", new { name = "Bob" }, "Hi Eve" };
            yield return new object[] { "{{#> missing}}fallback{{/missing}}", new { }, "fallback" };
            yield return new object[] { "{{#*inline \"item\"}}<{{this}}>{{/inline}}{{#each items}}{{> item}}{{/each}}", new { items = new[] { 1, 2 } }, "<1><2>" };
            yield return new object[] { "{{#each outer}}{{#each inner}}{{../name}}{{this}}{{/each}}{{/each}}", new { outer = new[] { new { name = "n", inner = new[] { 1, 2 } } } }, "n1n2" };
        }

        [Theory]
        [MemberData(nameof(Templates))]
        public void RendersSameOutputWhenExpressionsAreInterpreted(string source, object data, string expected)
        {
            var compiled = CreateEnvironment(interpret: false, out _);
            var interpreted = CreateEnvironment(interpret: true, out var interpreter);

            var compiledResult = compiled.Compile(source)(data);
            var interpretedResult = interpreted.Compile(source)(data);

            Assert.Equal(expected, compiledResult);
            Assert.Equal(expected, interpretedResult);
            Assert.True(interpreter!.Compilations > 0, "the interpreting expression compiler was not used");
        }

        [Fact]
        public void CompileTimeErrorsAreReportedWhenExpressionsAreInterpreted()
        {
            const string source = "{{#if x}}unterminated";
            var compiled = CreateEnvironment(interpret: false, out _);
            var interpreted = CreateEnvironment(interpret: true, out _);

            var compiledError = Record.Exception(() => compiled.Compile(source));
            var interpretedError = Record.Exception(() => interpreted.Compile(source));

            Assert.IsType<HandlebarsCompilerException>(compiledError);
            Assert.IsType<HandlebarsCompilerException>(interpretedError);
            Assert.Equal(compiledError.Message, interpretedError.Message);
        }

        private static IHandlebars CreateEnvironment(bool interpret, out InterpretingExpressionCompiler? interpreter)
        {
            var handlebars = Handlebars.Create();
            interpreter = null;
            if (interpret)
            {
                var feature = new InterpretingCompilerFeature();
                interpreter = feature.Compiler;
                handlebars.Configuration.CompileTimeConfiguration.Features.Add(new FeatureFactory(feature));
            }

            handlebars.RegisterHelper("concat", (context, arguments) => string.Concat(arguments[0], arguments[1]));
            handlebars.RegisterHelper("shout", (context, arguments) => arguments[0]?.ToString()?.ToUpperInvariant());
            handlebars.RegisterHelper("wrap", (output, options, context, arguments) =>
            {
                output.WriteSafeString("[");
                options.Template(output, context);
                output.WriteSafeString("]");
            });
            handlebars.RegisterTemplate("greeting", "Hi {{name}}");

            return handlebars;
        }

        private sealed class FeatureFactory : IFeatureFactory
        {
            private readonly IFeature _feature;
            public FeatureFactory(IFeature feature) => _feature = feature;
            public IFeature CreateFeature() => _feature;
        }

        // Registered without a FeatureOrder, so it runs after the built-in compiler feature and replaces it.
        private sealed class InterpretingCompilerFeature : IFeature
        {
            public InterpretingExpressionCompiler Compiler { get; } = new InterpretingExpressionCompiler();

            public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
            {
                Compiler.Middlewares = configuration.ExpressionMiddlewares;
                configuration.ExpressionCompiler = Compiler;
            }

            public void CompilationCompleted()
            {
            }
        }

        private sealed class InterpretingExpressionCompiler : IExpressionCompiler
        {
            public IReadOnlyList<IExpressionMiddleware> Middlewares { get; set; } = Array.Empty<IExpressionMiddleware>();

            public int Compilations { get; private set; }

            public T Compile<T>(Expression<T> expression) where T : class, Delegate
            {
                for (var index = 0; index < Middlewares.Count; index++)
                {
                    expression = Middlewares[index].Invoke(expression);
                }

                Compilations++;
                return expression.Compile(preferInterpretation: true);
            }
        }
    }
}
