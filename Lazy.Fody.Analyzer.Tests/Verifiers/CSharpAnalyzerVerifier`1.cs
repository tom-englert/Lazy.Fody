using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;

namespace Lazy.Fody.Analyzer.Tests.Verifiers
{
    public static partial class CSharpAnalyzerVerifier<TAnalyzer, TSystemAnalyzer>
    {
        public static async Task VerifyAnalyzerAsync(string source, ICollection<DiagnosticResult>? diagnostics = null)
        {
            diagnostics ??= Array.Empty<DiagnosticResult>();

            var test = new Test(source, diagnostics)
            {
                ReportSuppressedDiagnostics = true
            };

            test.AddReference(typeof(LazyAttribute).Assembly);

            await test.RunAsync(CancellationToken.None);
        }
    }
}
