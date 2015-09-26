namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    public class TestType
    {
        public string fieldStr;
        public int fieldInt;
        public double? fieldNullableDouble;
        public object fieldObj { get; set; }

        protected object protectedField;
        private object privateField;
        internal object internalField;

        public string TestStr { get; set; }

        public int TestInt { get; set; }

        public double? TestNullableDouble { get; set; }

        public object TestObject { get; set; }
    }
}