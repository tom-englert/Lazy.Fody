using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Testing;

using Xunit;

// using VerifyCS = Lazy.Fody.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Lazy.Fody.Analyzer.LazyDiagnosticSuppressor>;
using VerifyCS = Lazy.Fody.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<
        Lazy.Fody.Analyzer.LazyDiagnosticSuppressor,
        Microsoft.CodeQuality.Analyzers.QualityGuidelines.MarkMembersAsStaticAnalyzer>;

namespace Lazy.Fody.Analyzer.Tests
{
    public class SuppressorTests
    {
        [Fact]
        public async Task Test()
        {
            var test = @"
using Lazy;

class Test
{
    [Lazy]
    public Test {|#0:Property|} => new Test();
}";

            var suppressed = new[]
            {
                DiagnosticResult.CompilerError("CA1822").WithLocation(0)
            };

            await VerifyCS.VerifySuppressorAsync(test, suppressed);
        }
    }
}
