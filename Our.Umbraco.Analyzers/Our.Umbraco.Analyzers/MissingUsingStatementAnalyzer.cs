using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Headers;

namespace Our.Umbraco.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingUsingStatementAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UMB1010";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MissingUsingStatementAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MissingUsingStatementAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MissingUsingStatementAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Umbraco";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            context.RegisterCodeBlockAction(AnalyzeCodeBlock);
        }

        private void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var descendents = context.CodeBlock.DescendantNodes();

            var declarations = descendents.Where(n => 
                (n.Kind() == SyntaxKind.LocalDeclarationStatement || n.Kind() == SyntaxKind.ExpressionStatement) &&
                n.GetText().ToString().Contains("EnsureUmbracoContext")
            );

            var summary = declarations.Select(n => $"{n.Kind()}: {n}").ToArray();
            foreach (var declaration in declarations)
            {
                var text = declaration.GetText().ToString().Trim();
                if (!text.StartsWith("using "))
                {
                    var location = declaration.GetLocation();

                    var diagnostic = Diagnostic.Create(Rule, location, "EnsureUmbracoContext");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
