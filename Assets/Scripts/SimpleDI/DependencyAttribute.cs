using System;

namespace SimpleDI
{
    [AttributeUsage(
        AttributeTargets.Constructor |
        AttributeTargets.Method |
        AttributeTargets.Field |
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class DependencyAttribute : Attribute
    {
    }
}