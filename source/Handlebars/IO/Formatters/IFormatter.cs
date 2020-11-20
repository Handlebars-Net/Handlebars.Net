namespace HandlebarsDotNet.IO
{
    public interface IFormatter
    {
        void Format<T>(T value, in EncodedTextWriter writer);
    }
}