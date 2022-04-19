using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.String;

namespace Misbat.FastEnumNames.Test;

[TestClass]
public class FastNamedAnalyzerTest
{
    private const string UsingStatement = "using Misbat.FastEnumNames.Attributes;";

    [TestMethod]
    public async Task DuplicateEnumValueReportsWarning()
    {
        const string source = UsingStatement + @"
[FastNamed]
enum TestEnum
{
    Test1 = 0,
    Test2 = 0
}";
        string[] diagnosticIds = await CodeAnalysis.GetDiagnosticIds(source, new FastNamedAnalyzer());
        Console.WriteLine($"encountered diagnostic ids: {Join(", ", diagnosticIds)}");
        Assert.IsTrue(diagnosticIds.Contains(Diagnostics.FastEnumMembersShareValue.Id));
    }
}