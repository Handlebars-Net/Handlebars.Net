using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicViewModel : DynamicObject
    {
        /// <summary>
        /// 
        /// </summary>
        public object[] Objects { get; set; }

        private static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance |
                                                            BindingFlags.IgnoreCase;

        /// <summary>
        /// 
        /// </summary>
        public DynamicViewModel(params object[] objects)
        {
            Objects = objects;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Objects.Select(o => o.GetType())
                .SelectMany(t => t.GetMembers(BindingFlags))
                .Select(m => m.Name);
        }

        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            foreach (var target in Objects)
            {
                var member = target.GetType()
                    .GetMember(binder.Name, BindingFlags);

                if (member.Length > 0)
                {
                    if (member[0] is PropertyInfo)
                    {
                        result = ((PropertyInfo) member[0]).GetValue(target, null);
                        return true;
                    }
                    if (member[0] is FieldInfo)
                    {
                        result = ((FieldInfo) member[0]).GetValue(target);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}