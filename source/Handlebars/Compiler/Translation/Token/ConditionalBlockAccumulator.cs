using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Handlebars.Compiler.Lexer;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class ConditionalBlockAccumulator : TokenConverter
	{
        private readonly string[] conditionalNames = new string[] { "if", "unless" };
        public static IEnumerable<object> Accumulate(
            IEnumerable<object> tokens,
            HandlebarsConfiguration configuration)
        {
            return new ConditionalBlockAccumulator(configuration).ConvertTokens(tokens).ToList();
        }

        private readonly HandlebarsConfiguration _configuration;

        private ConditionalBlockAccumulator(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(IsConditionalBlock(item))
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

        private Expression AccumulateBlock(
            IEnumerator<object> enumerator, 
            HelperExpression startingNode)
        {
            BlockExpression accumulatedExpression = Expression.Block(
                new Expression[] { Expression.Empty() });
            var body = new List<Expression>();
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(IsConditionalBlock(item))
                {
                    item = UnwrapStatement(item);
                    if(((HelperExpression)item).Arguments.Count() > 1)
                    {
                        throw new HandlebarsCompilerException("{{if}} expression can only have one argument");
                    }
                    else if(((HelperExpression)item).Arguments.Count() == 0)
                    {
                        throw new HandlebarsCompilerException("{{if}} expression must have a boolean argument");
                    }
                    body.Add(AccumulateBlock(enumerator, (HelperExpression)item));
                }
                else if(IsElseBlock(item, startingNode))
                {
                    accumulatedExpression = AppendToBlock(accumulatedExpression, CreateIfBlock(startingNode, body));
                    body = new List<Expression>();
                    startingNode = HandlebarsExpression.Helper(
                        startingNode.HelperName,
                        new[] { Expression.Not(Expression.Convert(startingNode.Arguments.Single(), typeof(bool))) });
                }
                else if(IsBlockCloseNode(item, startingNode))
                {
                    accumulatedExpression = AppendToBlock(accumulatedExpression, CreateIfBlock(startingNode, body));
                    return accumulatedExpression;
                }
                else
                {
                    body.Add((Expression)item);
                }
            }
            throw new HandlebarsCompilerException("Reached end of template before block expression was closed");
        }

        private static BlockExpression AppendToBlock(BlockExpression block, Expression newBodyElement)
        {
            return Expression.Block(
                block.Expressions.Concat(new [] { newBodyElement }));
        }

        private static Expression CreateIfBlock(HelperExpression startingNode, IEnumerable<Expression> body)
        {
            var condition = Expression.Convert(startingNode.Arguments.Single(), typeof(bool));
            if(startingNode.HelperName.Replace("#", "") == "if")
            {
                return Expression.IfThen(condition, Expression.Block(body));
            }
            else if(startingNode.HelperName.Replace("#", "") == "unless")
            {
                return Expression.IfThen(Expression.Not(condition), Expression.Block(body));
            }
            else
            {
                throw new HandlebarsCompilerException(string.Format(
                    "Tried to create a conditional expression for '{0}'", startingNode.HelperName));
            }
        }

        private bool IsElseBlock(object item, HelperExpression startingNode)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }

        private bool IsBlockCloseNode(object item, HelperExpression startingNode)
        {
            item = UnwrapStatement(item);
            var blockName = startingNode.HelperName.Replace("#", "");
            return item is PathExpression && ((PathExpression)item).Path == "/" + blockName;
        }

        private bool IsConditionalBlock(object item)
        {
            item = UnwrapStatement(item);
            return (item is HelperExpression) && conditionalNames.Contains(((HelperExpression)item).HelperName.Replace("#", ""));
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

