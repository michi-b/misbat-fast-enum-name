using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Misbat.FastEnumNames;

[DiagnosticAnalyzer("C#")]
[UsedImplicitly]
public class NamedEnumAnalyzer : DiagnosticAnalyzer
{

    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    [SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Analyzer does not handle target-typed new")]
    private static readonly DiagnosticDescriptor EnumMembersMustNotShareValuesRule = new DiagnosticDescriptor(
        "NE0001",
        "Multiple members of fast named enum share the same value",
        "the member '{0}' of the fast named enum '{1}' has the value '{2}', which is already taken by the member '{3}'",
        "Function",
        DiagnosticSeverity.Warning,
        true,
        "Different fast named enum members with a shared value can't have their name reliably deduced.");


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(EnumMembersMustNotShareValuesRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var enumSymbol = (INamedTypeSymbol)context.Symbol;
        var underlyingType = enumSymbol.EnumUnderlyingType;

        //if symbol is not an enum declaration, return
        if (underlyingType == null)
        {
            return;
        }

        //if enum does not have the fast named attribute, return
        if (!enumSymbol.GetAttributes()
                .Any(ad => ad.IsClass("global::Misbat.FastEnumNames.FastNamedAttribute")))
        {
            return;
        }


        var values = new Dictionary<object, IFieldSymbol>();

        foreach (var enumMemberSymbol in enumSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            object constantValue = enumMemberSymbol.ConstantValue;

            if (values.TryGetValue(constantValue, out var otherMemberWithSameValue))
            {
                var declarationLocations = enumMemberSymbol.GetDeclarationLocations().ToArray();

                if (declarationLocations.Length > 0)
                {
                    foreach (var location in declarationLocations)
                    {
                        ReportEnumMembersShareValue(context, 
                            enumSymbol, 
                            enumMemberSymbol, 
                            otherMemberWithSameValue,
                            constantValue, 
                            location);
                    }
                }
                else
                {
                    ReportEnumMembersShareValue(context, 
                        enumSymbol, 
                        enumMemberSymbol, 
                        otherMemberWithSameValue,
                        constantValue, 
                        Location.None);
                }
            }

            values[constantValue] = enumMemberSymbol;
        }
    }

    private static void ReportEnumMembersShareValue(SymbolAnalysisContext context,
        ISymbol enumSymbol,
        ISymbol enumMemberSymbol,
        ISymbol otherMemberWithSameValue,
        object constantValue,
        Location location
    )
    {
        context.ReportDiagnostic(Diagnostic.Create(EnumMembersMustNotShareValuesRule,
            location,
            enumMemberSymbol.Name,
            enumSymbol.Name,
            constantValue, otherMemberWithSameValue.Name));
    }
}