using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Handlebars.Compiler.Lexer;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class BlockHelperAccumulator : TokenConverter
	{
        public static IEnumerable<object> Accumulate(
            IEnumerable<object> tokens,
            HandlebarsConfiguration configuration)
        {
            return new BlockHelperAccumulator(configuration).ConvertTokens(tokens).ToList();
        }

        private readonly HandlebarsConfiguration _configuration;

        private BlockHelperAccumulator(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(IsBlockHelper(item))
                {
                    item = UnwrapStatement(item);
                    yield return AccumulateBlock(enumerator, (HelperExpression)item);
                }
                else
                {
                    yield return item;
                }
            }
        }

        private BlockHelperExpression AccumulateBlock(
            IEnumerator<object> enumerator, 
            HelperExpression startingNode)
        {
            var body = new List<Expression>();
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(IsBlockHelper(item))
                {
                    item = UnwrapStatement(item);
                    body.Add(AccumulateBlock(enumerator, (HelperExpression)item));
                }
                else if(IsBlockCloseNode(item, startingNode))
                {
                    return HandlebarsExpression.BlockHelper(
                        startingNode.HelperName,
                        startingNode.Arguments,
                        Expression.Block(body));
                }
                else
                {
                    body.Add((Expression)item);
                }
            }
            throw new HandlebarsCompilerException("Reached end of template before block expression was closed");
        }

        private bool IsBlockCloseNode(object item, HelperExpression startingNode)
        {
            item = UnwrapStatement(item);
            var helperName = startingNode.HelperName.Replace("#", "");
            return item is PathExpression && ((PathExpression)item).Path == "/" + helperName;
        }

        private bool IsBlockHelper(object item)
        {
            item = UnwrapStatement(item);
            return (item is HelperExpression) && _configuration.BlockHelpers.ContainsKey(((HelperExpression)item).HelperName);
        }

        private object UnwrapStatement(object item)
        {
            if(item is StatementExpression)
            {
                return ((StatementExpression)item).Body;
            }
            else
            {
                return item;
            }
        }
	}
}

