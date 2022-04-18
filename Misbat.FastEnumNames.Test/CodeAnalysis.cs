using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Misbat.FastEnumNames.Test;

public static class CodeAnalysis
{
    public static async Task<string[]> GetDiagnosticIds(string source, params DiagnosticAnalyzer[] analyzers)
    {
        var analyzerArray = analyzers.ToImmutableArray();

        using Workspace workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution;

        List<MetadataReference> references = new();
        string msCoreLibPath = typeof(object).Assembly.Location;
        //string msCoreLibPathFolder = Path.GetDirectoryName(msCoreLibPath)!;

        references.Add(MetadataReference.CreateFromFile(msCoreLibPath));

        var projectId = ProjectId.CreateNewId();
        var projectInfo = ProjectInfo.Create(projectId,
            VersionStamp.Default,
            "Test",
            "Test",
            "C#",
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            parseOptions: CSharpParseOptions.Default,
            metadataReferences: references);

        var project = solution.AddProject(projectInfo).GetProject(projectId)!;

        var document = project.AddDocument("Test.cs", source);

        var compilation = await document.Project.GetCompilationAsync();
        var compilationWithAnalyzer = compilation!.WithAnalyzers(analyzerArray);
        var diagnostics = await compilationWithAnalyzer.GetAnalyzerDiagnosticsAsync(analyzerArray, CancellationToken.None);
        //var diagnostics = await compilationWithAnalyzer.GetAllDiagnosticsAsync(CancellationToken.None);
        return diagnostics.Select(d => d.Id).ToArray();
    }
}