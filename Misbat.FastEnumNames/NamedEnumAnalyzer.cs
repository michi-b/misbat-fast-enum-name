using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Misbat.FastEnumNames;

[DiagnosticAnalyzer("C#")]
public class NamedEnumAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(UnderlyingEnumTypeMustBeIntRule,
            EnumMembersMustNotShareValuesRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext symbolAnalysisContext)
    {
        var enumSymbol = (INamedTypeSymbol)symbolAnalysisContext.Symbol;
        var underlyingType = enumSymbol.EnumUnderlyingType;

        //if symbol is not an enum declaration, return
        if (underlyingType == null) return;

        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }

        //if enum does not have the fast named attribute, return
        if (!enumSymbol.GetAttributes()
                .Any(ad => ad.IsClass("global::Misbat.FastEnumNames.FastNamedAttribute")))
            return;

        if (!underlyingType.HasFullyQualifiedName("global::System.Int32"))
        {
            symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(UnderlyingEnumTypeMustBeIntRule,
                Location.None,
                enumSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                underlyingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            return;
        }

        Console.WriteLine("HACK");
        //var attributeSyntaxList = enumSymbol.ChildNodes().OfType<AttributeListSyntax>() //from all attribute lists
        //    .Select(als => als.ChildNodes().OfType<AttributeSyntax>()) //select all attributes
        //    .Aggregate(Enumerable.Concat); //and concatenate them into a single list 

        //var semanticModel = context.SemanticModel;

        //foreach (var attributeSyntax in attributeSyntaxList)
        //{
        //    var attributeSymbol = semanticModel.GetSymbolInfo(attributeSyntax.Name).Symbol as Microsoft.CodeAnalysis.ISymbol;
        //    Debug.Assert(attributeSymbol != null, nameof(attributeSymbol) + " != null");
        //    if (!Debugger.IsAttached)
        //    {
        //        Debugger.Launch();
        //    }
        //    if (attributeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) !=
        //        "Misbat.FastEnumNames.FastNamedAttribute") continue;
        //    if (!Debugger.IsAttached)
        //    {
        //        Debugger.Launch();
        //    }
        //}
    }

//analyzer analyzer complains when target typed new is used here
#pragma warning disable IDE0090 // Use 'new(...)'
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

    private static readonly DiagnosticDescriptor UnderlyingEnumTypeMustBeIntRule = new DiagnosticDescriptor(
        "NE0001",
        "Fast named enum has a different underlying type than int",
        "The fast named enum {{{0}}} has a different type than System.Int32 ({1}), which is not allowed",
        "Function",
        DiagnosticSeverity.Warning,
        true,
        "For the sake of simplicity, only enums with the underlying type System.Int32 can be fast named.");

    private static readonly DiagnosticDescriptor EnumMembersMustNotShareValuesRule = new DiagnosticDescriptor(
        "NE0002",
        "Multiple members of fast named enum share the same value",
        "the members {{{0}}} of the fast named enum {{{1}}} share the same value",
        "Function",
        DiagnosticSeverity.Warning,
        true,
        "Different fast named enum members with a shared value can't have their name reliably deduced.");

    // ReSharper restore ArrangeObjectCreationWhenTypeEvident
#pragma warning restore IDE0090 // Use 'new(...)'
}