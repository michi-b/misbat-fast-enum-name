using Microsoft.CodeAnalysis;

namespace Misbat.FastEnumNames;

internal static class NamedTypeSymbolExtensions
{
    public static bool HasFullyQualifiedName(this INamedTypeSymbol target, string fullyQualifiedName)
    {
        return target.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedName;
    }
}