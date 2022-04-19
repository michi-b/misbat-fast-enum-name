using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Misbat.FastEnumNames;

public static class Diagnostics
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    [SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Analyzer does not handle target-typed new")]
    public static readonly DiagnosticDescriptor FastEnumMembersShareValue = new DiagnosticDescriptor(
        "NE0001",
        "Multiple members of fast named enum share the same value",
        "the member '{0}' of the fast named enum '{1}' has the value '{2}', which is already taken by the member '{3}'",
        "Function",
        DiagnosticSeverity.Warning,
        true,
        "Different fast named enum members with a shared value can't have their name reliably deduced.");

    public static void ReportFastEnumMembersShareValue(SymbolAnalysisContext context,
        ISymbol enumSymbol,
        ISymbol enumMemberSymbol,
        ISymbol otherMemberWithSameValue,
        object constantValue,
        Location location
    )
    {
        context.ReportDiagnostic(Diagnostic.Create(FastEnumMembersShareValue,
            location,
            enumMemberSymbol.Name,
            enumSymbol.Name,
            constantValue, otherMemberWithSameValue.Name));
    }
}