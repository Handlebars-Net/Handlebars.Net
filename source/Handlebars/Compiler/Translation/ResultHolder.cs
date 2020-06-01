namespace HandlebarsDotNet.Compiler
{
    internal struct ResultHolder
    {
        public readonly bool Success;
        public readonly object Value;

        public ResultHolder(bool success, object value)
        {
            Success = success;
            Value = value;
        }
    }
}