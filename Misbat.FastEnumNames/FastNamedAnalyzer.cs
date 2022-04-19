using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Misbat.FastEnumNames;

[DiagnosticAnalyzer("C#")]
[UsedImplicitly]
public class FastNamedAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Diagnostics.FastEnumMembersShareValue);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
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
                .Any(ad => ad.IsClass("global::Misbat.FastEnumNames.Attributes.FastNamedAttribute")))
        {
            return;
        }


        var values = new Dictionary<object, IFieldSymbol>();

        foreach (var enumMemberSymbol in enumSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            object constantValue = enumMemberSymbol.ConstantValue;

            if (values.TryGetValue(constantValue!, out var otherMemberWithSameValue))
            {
                var declarationLocations = enumMemberSymbol.GetDeclarationLocations().ToArray();

                if (declarationLocations.Length > 0)
                {
                    foreach (var location in declarationLocations)
                    {
                        Diagnostics.ReportFastEnumMembersShareValue(context,
                            enumSymbol,
                            enumMemberSymbol,
                            otherMemberWithSameValue,
                            constantValue,
                            location);
                    }
                }
            }

            values[constantValue] = enumMemberSymbol;
        }
    }
}