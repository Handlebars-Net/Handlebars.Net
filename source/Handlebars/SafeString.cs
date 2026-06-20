namespace HandlebarsDotNet
{
    /// <summary>
    /// Wraps a string value that has already been HTML-encoded (or is intentionally unencoded HTML).
    /// When written to an <see cref="EncodedTextWriter"/>, the content is passed through without
    /// additional encoding. This is the return-value counterpart of
    /// <see cref="HandlebarsExtensions.WriteSafeString(in EncodedTextWriter, string)"/>.
    /// </summary>
    internal sealed class SafeString
    {
        public readonly string Value;

        public SafeString(string value) => Value = value;

        public override string ToString() => Value;
    }
}
