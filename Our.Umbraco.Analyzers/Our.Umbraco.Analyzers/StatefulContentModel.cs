using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Our.Umbraco.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StatefulContentModel : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UMB1000";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.StatefulContentModelAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.StatefulContentModelAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.StatefulContentModelAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Umbraco";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, ImmutableArray.Create(SyntaxKind.PropertyDeclaration));
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Kind() != SyntaxKind.PropertyDeclaration)
            {
                return;
            }

            var propertyDeclaration = (PropertyDeclarationSyntax) context.Node;
            
            var namespaceName = context.ContainingSymbol.ContainingNamespace.Name;
            if (propertyDeclaration.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration) &&
                namespaceName.Equals("PublishedModels"))
            {
                var location = context.Node.GetLocation();
                
                var diagnostic = Diagnostic.Create(Rule, location, propertyDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
