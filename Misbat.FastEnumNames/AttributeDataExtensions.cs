using Microsoft.CodeAnalysis;

namespace Misbat.FastEnumNames;

internal static class AttributeDataExtensions
{
    public static bool IsClass(this AttributeData target, string fullyQualifiedAttributeClassName)
    {
        var attributeClass = target.AttributeClass;
        return attributeClass.HasFullyQualifiedName(fullyQualifiedAttributeClassName);
    }
}