namespace HandlebarsDotNet.StringUtils
{
    internal interface IStringWrapper
    {
        int Count { get; }
        
        char this[int index] { get; }
    }
}