using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Testing;

using Xunit;

using VerifyCS = Lazy.Fody.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<
        Lazy.Fody.Analyzer.LazyDiagnosticSuppressor,
        Microsoft.CodeQuality.Analyzers.QualityGuidelines.MarkMembersAsStaticAnalyzer>;

namespace Lazy.Fody.Analyzer.Tests
{
    public class SuppressorTests
    {
        [Fact]
        public async Task Test1()
        {
            var test = @"
using Lazy;

class Test
{
    [Lazy]
    public Test {|#0:Property|} => new Test();
}";

            var results = new[]
            {
                DiagnosticResult.CompilerError("CA1822").WithLocation(0).WithIsSuppressed(true)
            };

            await VerifyCS.VerifyAnalyzerAsync(test, results);
        }

        [Fact]
        public async Task Test2()
        {
            var test = @"
using Lazy;

class Test
{
    [Lazy]
    public Test {|#0:Property0|} => new Test();
    public Test {|#1:Property1|} => new Test();
}";

            var expected = new[]
            {
                DiagnosticResult.CompilerError("CA1822").WithLocation(0).WithIsSuppressed(true),
                DiagnosticResult.CompilerError("CA1822").WithLocation(1).WithIsSuppressed(false)
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task Test3()
        {
            var test = @"
using Lazy;

class Test
{
    public Test {|#0:Property0|} => new Test();
    [Lazy]
    public Test {|#1:Property1|} => new Test();
}";

            var expected = new[]
            {
                DiagnosticResult.CompilerError("CA1822").WithLocation(0).WithIsSuppressed(false),
                DiagnosticResult.CompilerError("CA1822").WithLocation(1).WithIsSuppressed(true)
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task Test4()
        {
            var test = @"
class Test
{
    [Lazy.Lazy]
    public Test {|#0:Property|} => new Test();
}";

            var expected = new[]
            {
                DiagnosticResult.CompilerError("CA1822").WithLocation(0).WithIsSuppressed(true)
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

    }
}
