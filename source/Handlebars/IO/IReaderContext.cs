namespace HandlebarsDotNet
{
    internal interface IReaderContext
    {
        int LineNumber { get; set; }
        int CharNumber { get; set; }
    }
}