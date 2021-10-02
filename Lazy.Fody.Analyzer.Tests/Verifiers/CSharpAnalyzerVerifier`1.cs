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
        public static async Task VerifyAnalyzerAsync(string source, ICollection<DiagnosticResult>? diagnostics = null, IDictionary<string, ReportDiagnostic>? diagnosticOptions = null)
        {
            diagnostics ??= Array.Empty<DiagnosticResult>();

            var test1 = new Test(source, diagnostics, diagnosticOptions);

            await test1.RunAsync(CancellationToken.None);
        }
    }
}
