using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Lazy.Fody.Analyzer.Tests.Verifiers
{
    public static partial class CSharpAnalyzerVerifier<TAnalyzer, TSystemAnalyzer>
        where TAnalyzer : DiagnosticSuppressor, new()
        where TSystemAnalyzer : DiagnosticAnalyzer, new()
    {
        private class Test : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            private readonly TSystemAnalyzer _systemAnalyzer = new();

            public Test(string testCode, IEnumerable<DiagnosticResult> expectedDiagnostics)
            {
                ExpectedDiagnostics.AddRange(expectedDiagnostics);
                TestCode = testCode;
                TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;

                var diagnosticOptions = _systemAnalyzer.SupportedDiagnostics
                    .Select(item => item.Id)
                    .ToDictionary(item => item, _ => ReportDiagnostic.Error);

                SolutionTransforms.Add((solution, projectId) =>
                {
                    solution = solution.AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(LazyAttribute).Assembly.Location));

                    var project = solution
                        .GetProject(projectId);

                    var compilationOptions = (CSharpCompilationOptions)project!.CompilationOptions!;

                    compilationOptions = compilationOptions
                        .WithGeneralDiagnosticOption(ReportDiagnostic.Error)
                        .WithSpecificDiagnosticOptions(diagnosticOptions)
                        .WithNullableContextOptions(NullableContextOptions.Enable);

                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                    return solution;
                });
            }

            protected override ParseOptions CreateParseOptions()
            {
                return new CSharpParseOptions(LanguageVersion.CSharp11, DocumentationMode.Diagnose);
            }

            protected override CompilationWithAnalyzers CreateCompilationWithAnalyzers(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerOptions options, CancellationToken cancellationToken)
            {
                return compilation.WithAnalyzers(analyzers, new CompilationWithAnalyzersOptions(options, null, true, false, true));
            }

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                return base.GetDiagnosticAnalyzers().Concat(new[] { _systemAnalyzer });
            }
        }
    }
}
