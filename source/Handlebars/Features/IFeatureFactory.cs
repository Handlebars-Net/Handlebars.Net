namespace HandlebarsDotNet.Features
{
    
    public interface IFeatureFactory
    {
        /// <summary>
        /// Creates new <see cref="IFeature"/> for each template
        /// </summary>
        /// <returns></returns>
        IFeature CreateFeature();
    }
}