using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Lazy.Fody.Analyzer.Tests.Verifiers
{
    public static partial class CSharpAnalyzerVerifier<TAnalyzer, TSystemAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TSystemAnalyzer : DiagnosticAnalyzer, new()
    {
        public static async Task VerifySuppressorAsync(string source, ICollection<DiagnosticResult>? suppressed = null, ICollection<DiagnosticResult>? permanent = null)
        {
            suppressed ??= Array.Empty<DiagnosticResult>();
            permanent ??= Array.Empty<DiagnosticResult>();

            var test1 = new Test(source, false);

            test1.ExpectedDiagnostics.AddRange(permanent);
            test1.ExpectedDiagnostics.AddRange(suppressed);
            await test1.RunAsync(CancellationToken.None);

            var test2 = new Test(source, true);

            test2.ExpectedDiagnostics.AddRange(permanent);
            await test2.RunAsync(CancellationToken.None);
        }

        public static async Task VerifyAnalyzerAsync(string source, ICollection<DiagnosticResult> diagnostics, IDictionary<string, ReportDiagnostic> diagnosticOptions = null)
        {
            diagnostics ??= Array.Empty<DiagnosticResult>();

            var test1 = new Test(source, false, diagnosticOptions);

            test1.ExpectedDiagnostics.AddRange(diagnostics);
            await test1.RunAsync(CancellationToken.None);
        }
    }
}
