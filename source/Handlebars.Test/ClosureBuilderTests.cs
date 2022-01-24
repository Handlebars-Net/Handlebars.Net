using System.Collections.Generic;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class ClosureBuilderTests
    {
        [Fact]
        public void GeneratesClosureWithOverflow()
        {
            using var builder = ClosureBuilder.Create();

            var paths = GeneratePaths(builder, 6);
            var helpers = GenerateHelpers(builder, 6);
            var blockHelpers = GenerateBlockHelpers(builder, 6);
            var decoratorDelegates = GenerateDecoratorDelegates(builder, 6);
            var others = GenerateOther(builder, 6);

            _ = builder.Build(out var closure);
            
            Assert.Equal(paths[0], closure.PI0);
            Assert.Equal(paths[3], closure.PI3);
            Assert.Equal(paths[5], closure.PIA[1]);
            
            Assert.Equal(helpers[0], closure.HD0);
            Assert.Equal(helpers[3], closure.HD3);
            Assert.Equal(helpers[5], closure.HDA[1]);
            
            Assert.Equal(blockHelpers[0], closure.BHD0);
            Assert.Equal(blockHelpers[3], closure.BHD3);
            Assert.Equal(blockHelpers[5], closure.BHDA[1]);
            
            Assert.Equal(decoratorDelegates[0], closure.DDD0);
            Assert.Equal(decoratorDelegates[3], closure.DDD3);
            Assert.Equal(decoratorDelegates[5], closure.DDDA[1]);
            
            Assert.Equal(others[0], closure.A[0]);
            Assert.Equal(others[3], closure.A[3]);
            Assert.Equal(others[5], closure.A[5]);
        }
        
        [Fact]
        public void GeneratesClosureWithoutOverflow()
        {
            using var builder = ClosureBuilder.Create();

            var paths = GeneratePaths(builder, 2);
            var helpers = GenerateHelpers(builder, 2);
            var blockHelpers = GenerateBlockHelpers(builder, 2);
            var decorators = GenerateDecoratorDelegates(builder, 2);
            var others = GenerateOther(builder, 2);

            _ = builder.Build(out var closure);

            Assert.Equal(paths[0], closure.PI0);
            Assert.Equal(paths[1], closure.PI1);
            Assert.Null(closure.PIA);
            
            Assert.Equal(helpers[0], closure.HD0);
            Assert.Equal(helpers[1], closure.HD1);
            Assert.Null(closure.HDA);
            
            Assert.Equal(blockHelpers[0], closure.BHD0);
            Assert.Equal(blockHelpers[1], closure.BHD1);
            Assert.Null(closure.BHDA);
            
            Assert.Equal(decorators[0], closure.DDD0);
            Assert.Equal(decorators[1], closure.DDD1);
            Assert.Null(closure.DDD2);
            Assert.Null(closure.BHDA);
            
            Assert.Equal(others[0], closure.A[0]);
            Assert.Equal(others[1], closure.A[1]);
            Assert.Equal(2, closure.A.Length);
        }

        private static List<object> GenerateOther(ClosureBuilder builder, int count)
        {
            var others = new List<object>();
            for (int i = 0; i < count; i++)
            {
                var other = new object();
                builder.Add(Const(other));
                others.Add(other);
            }

            return others;
        }
        
        private static List<Ref<IHelperDescriptor<BlockHelperOptions>>> GenerateBlockHelpers(ClosureBuilder builder, int count)
        {
            var blockHelpers = new List<Ref<IHelperDescriptor<BlockHelperOptions>>>();
            for (int i = 0; i < count; i++)
            {
                var blockHelper = new Ref<IHelperDescriptor<BlockHelperOptions>>();
                builder.Add(Const(blockHelper));
                blockHelpers.Add(blockHelper);
            }

            return blockHelpers;
        }

        private static List<Ref<IHelperDescriptor<HelperOptions>>> GenerateHelpers(ClosureBuilder builder, int count)
        {
            var helpers = new List<Ref<IHelperDescriptor<HelperOptions>>>();
            for (int i = 0; i < count; i++)
            {
                var helper = new Ref<IHelperDescriptor<HelperOptions>>();
                builder.Add(Const(helper));
                helpers.Add(helper);
            }

            return helpers;
        }
        
        private static List<DecoratorDelegate> GenerateDecoratorDelegates(ClosureBuilder builder, int count)
        {
            var helpers = new List<DecoratorDelegate>();
            for (int i = 0; i < count; i++)
            {
                DecoratorDelegate helper = (in EncodedTextWriter writer, BindingContext context, TemplateDelegate function) => function;
                builder.Add(Const(helper));
                helpers.Add(helper);
            }

            return helpers;
        }

        private static List<PathInfo> GeneratePaths(ClosureBuilder builder, int count)
        {
            var paths = new List<PathInfo>();
            for (int i = 0; i < count; i++)
            {
                var pathInfo = PathInfo.Parse($"{i}");
                builder.Add(Const(pathInfo));
                paths.Add(pathInfo);
            }

            return paths;
        }

        private static ConstantExpression Const<T>(T value) => Expression.Constant(value, typeof(T));
    }
}