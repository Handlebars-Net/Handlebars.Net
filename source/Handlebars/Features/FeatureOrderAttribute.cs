using System;

namespace HandlebarsDotNet.Features
{
    internal class FeatureOrderAttribute : Attribute
    {
        public int Order { get; }

        public FeatureOrderAttribute(int order)
        {
            Order = order;
        }
    }
}