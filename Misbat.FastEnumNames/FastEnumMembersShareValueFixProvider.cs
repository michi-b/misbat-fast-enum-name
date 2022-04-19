using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Misbat.FastEnumNames;

[UsedImplicitly]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FastEnumMembersShareValueFixProvider))]
[Shared]
public class FastEnumMembersShareValueFixProvider : CodeFixProvider
{
    private const string RemoveInitializationTitle = "Remove initialization";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(Diagnostics.FastEnumMembersShareValue.Id);

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        Debug.Assert(root != null);
        foreach (var diagnostic in context.Diagnostics)
        {
            var span = diagnostic.Location.SourceSpan;
            if (span.IsEmpty)
            {
                continue;
            }

            var enumMemberDeclarationSyntax = root.FindNode(span, getInnermostNodeForTie: true)
                .AncestorsAndSelf()
                .OfType<EnumMemberDeclarationSyntax>()
                .First();

            context.RegisterCodeFix(CodeAction.Create(RemoveInitializationTitle,
                ct => RemoveInitialization(context.Document, enumMemberDeclarationSyntax, ct),
                RemoveInitializationTitle), diagnostic);
        }
    }

    private Task<Document> RemoveInitialization(Document document, EnumMemberDeclarationSyntax enumMemberDeclaration,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(document);
    }
}