using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.String;

namespace Misbat.FastEnumNames.Test;

[TestClass]
public class FastNamedAnalyzerTest
{
    [TestMethod]
    public async Task DuplicateEnumValueReportsWarning()
    {
        const string source = @"
[Misbat.FastEnumNames.FastNamed]
enum TestEnum
{
    Test1 = 0,
    Test2 = 0
}";
        string[] diagnosticIds = await CodeAnalysis.GetDiagnosticIds(source, new FastNamedAnalyzer());
        Console.WriteLine($"encountered diagnostic ids: {Join(", ", diagnosticIds)}");
        Assert.IsTrue(diagnosticIds.Contains(FastNamedAnalyzer.EnumMembersMustNotShareValuesRule.Id));
    }
}