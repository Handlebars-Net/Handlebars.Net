using System;

namespace HandlebarsDotNet.Features
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class FeatureOrderAttribute : Attribute
    {
        public int Order { get; }

        public FeatureOrderAttribute(int order)
        {
            Order = order;
        }
    }
}