using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

SyntaxTree tree = CSharpSyntaxTree.ParseText
(
    @"
class C
{
    void Method()
    {
    }

    void Method2()
    {
    }
}"
);

IEnumerable<Diagnostic> diagnostics = tree.GetDiagnostics().Where(n => n.Severity == DiagnosticSeverity.Error);

foreach (Diagnostic diagnostic in diagnostics)
{
    Console.WriteLine(diagnostic.GetMessage());
}

SyntaxNode syntaxRoot = tree.GetRoot();
MethodDeclarationSyntax methodSyntax = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
ClassDeclarationSyntax classSyntax = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
Console.WriteLine(methodSyntax.Identifier.ToString());
Console.WriteLine(classSyntax.Identifier.ToString());

var walker = new CustomWalker();
walker.Visit(syntaxRoot);


internal class CustomWalker : CSharpSyntaxWalker
{
    private static int _tabs;

    public CustomWalker() : base(SyntaxWalkerDepth.Token) { }

    public override void Visit(SyntaxNode? node)
    {
        _tabs++;
        string indents = new('\t', _tabs);
        Console.WriteLine(indents + node!.Kind());
        base.Visit(node);
        _tabs--;
    }

    public override void VisitToken(SyntaxToken token)
    {
        _tabs++;
        string indents = new('\t', _tabs);
        Console.WriteLine(indents + token);
        _tabs--;
    }


}