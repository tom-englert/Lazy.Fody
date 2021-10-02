using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Lazy.Fody.Analyzer.Tests.Verifiers
{
    public static partial class CSharpAnalyzerVerifier<TAnalyzer, TSystemAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TSystemAnalyzer : DiagnosticAnalyzer, new()
    {
        private class Test : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            private readonly bool _ignoreSuppressedDiagnostics;
            private readonly TSystemAnalyzer _systemAnalyzer = new TSystemAnalyzer();

            public Test(string testCode, bool ignoreSuppressedDiagnostics, IDictionary<string, ReportDiagnostic> diagnosticOptions = null)
            {
                _ignoreSuppressedDiagnostics = ignoreSuppressedDiagnostics;
                TestCode = testCode;
                TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;
                diagnosticOptions ??= _systemAnalyzer.SupportedDiagnostics
                    .Select(item => item.Id)
                    .ToDictionary(item => item, _ => ReportDiagnostic.Error);

                SolutionTransforms.Add((solution, projectId) =>
                {
                    solution = solution.AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(LazyAttribute).Assembly.Location));

                    var project = solution
                        .GetProject(projectId);

                    var compilationOptions = (CSharpCompilationOptions?)project
                        .CompilationOptions;

                    compilationOptions = compilationOptions
                        .WithGeneralDiagnosticOption(ReportDiagnostic.Error)
                        .WithSpecificDiagnosticOptions(diagnosticOptions)
                        .WithNullableContextOptions(NullableContextOptions.Enable);

                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                    return solution;
                });
            }

            protected override bool IsCompilerDiagnosticIncluded(Diagnostic diagnostic, CompilerDiagnostics compilerDiagnostics)
            {
                if (_ignoreSuppressedDiagnostics && diagnostic.IsSuppressed)
                    return false;

                return base.IsCompilerDiagnosticIncluded(diagnostic, compilerDiagnostics);
            }

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                return base.GetDiagnosticAnalyzers().Concat(new[] { _systemAnalyzer });
            }
        }
    }
}
