namespace HandlebarsDotNet.Compiler
{
    // Will be removed in next iterations
    internal readonly struct ResultHolder
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