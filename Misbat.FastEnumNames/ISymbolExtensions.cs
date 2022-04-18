using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Misbat.FastEnumNames;

internal static class ISymbolExtensions
{
    public static IEnumerable<Location> GetDeclarationLocations(this ISymbol target)
    {
        return target.DeclaringSyntaxReferences.Select(s => Location.Create(s.SyntaxTree, s.Span));
    }
}