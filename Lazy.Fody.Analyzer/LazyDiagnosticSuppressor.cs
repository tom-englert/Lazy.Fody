using System;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lazy.Fody.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LazyDiagnosticSuppressor : DiagnosticSuppressor
    {
        private static readonly string[] SupportedSuppressionIds = { "CA1822" };

        private static SuppressionDescriptor ToSuppressionDescriptor(string id) =>
            new("Lazy_" + id, id, $"Suppress {id}.");

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = SupportedSuppressionIds.Select(ToSuppressionDescriptor).ToImmutableArray();

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            var cancellationToken = context.CancellationToken;

            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                try
                {
                    var location = diagnostic.Location;
                    var sourceTree = location.SourceTree;
                    if (sourceTree == null)
                        continue;

                    var root = sourceTree.GetRoot(cancellationToken);

                    var sourceSpan = location.SourceSpan;
                    var elementNode = root.FindNode(sourceSpan);

                    if (elementNode is PropertyDeclarationSyntax propertyDeclaration)
                    {
                        if (propertyDeclaration.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString() == "Lazy"))
                        {
                            context.ReportSuppression(Suppression.Create(SupportedSuppressions[0], diagnostic));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // could not analyze, so just do not suppress anything.
                }
            }
        }
    }
}
