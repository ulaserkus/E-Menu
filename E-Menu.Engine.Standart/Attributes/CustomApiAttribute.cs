using E_Menu.Engine.Constants;
using System;

namespace E_Menu.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomApiAttribute : Attribute
    {
        public string Prefix { get; } = CustomApiConstants.Prefix;
        public string Description { get; }
        public string SolutionName { get; } = CustomApiConstants.SolutionUniqueName;
        public Type TargetType { get; }
        public bool ContainsInput { get; } = true;


        public string Name => TargetType?.Name;
        public string NamespaceName => TargetType?.Namespace;




        public CustomApiAttribute(
            string description,
            Type targetType)
        {
            Description = description;
            TargetType = targetType;
        }
    }
}
