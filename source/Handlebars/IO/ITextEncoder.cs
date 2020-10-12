namespace HandlebarsDotNet
{
    /// <summary>
    /// Encoder used for output encoding.
    /// </summary>
    public interface ITextEncoder
    {
        string Encode(string value);
    }
}