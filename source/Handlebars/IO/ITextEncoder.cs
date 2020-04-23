namespace HandlebarsDotNet
{
    /// <summary>
    /// Encoder used for output encoding.
    /// </summary>
    public interface ITextEncoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Encode(string value);
    }
}