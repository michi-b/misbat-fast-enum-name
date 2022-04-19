using Microsoft.CodeAnalysis;

namespace Misbat.FastEnumNames;

internal static class NamedTypeSymbolExtensions
{
    public static bool HasFullyQualifiedName(this INamedTypeSymbol target, string fullyQualifiedName)
    {
        string fullyQualifiedSymbolName = target.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return fullyQualifiedSymbolName == fullyQualifiedName;
    }
}