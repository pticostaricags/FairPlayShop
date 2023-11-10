using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace FairPlaySocial.Services.Generators
{
    [Generator]
    public class ConfigureModelsLocalizersIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
#endif
            // Do a simple filter for enums
            IncrementalValuesProvider<MethodDeclarationSyntax> classDeclarations =
                context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
                .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

            // Combine the selected interfaces with the `Compilation`
            IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax>)>
                compilationAndClasses
                = context.CompilationProvider.Combine(classDeclarations.Collect());

            // Generate the source using the compilation and classes
            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static MethodDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext generatorSyntaxContext)
        {
            var classDeclarationSyntax = generatorSyntaxContext.Node as MethodDeclarationSyntax;
            return classDeclarationSyntax!;
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
        {
            if (syntaxNode is MethodDeclarationSyntax classDeclarationSyntax)
            {
                foreach (var singleAttributeList in classDeclarationSyntax.AttributeLists)
                {
                    foreach (var singleAttribute in singleAttributeList.Attributes)
                    {
                        var identifierNameSyntax = (singleAttribute.Name) as IdentifierNameSyntax;
                        string identifierText = identifierNameSyntax!.Identifier.Text;
                        if (identifierText == "ConfigureModelsLocalizers")
                            return true;
                    }
                }
            }
            return false;
        }

        static void Execute(Compilation compilation,
            ImmutableArray<MethodDeclarationSyntax> classesDeclarationSyntax, SourceProductionContext context)
        {
            if (classesDeclarationSyntax.Length == 0)
                return;
            string assemblyName = compilation.AssemblyName!;
            string[] splittedAssemblyName = assemblyName.Split('.');
            string assemblyNameFirstPart = splittedAssemblyName[0];
            foreach (var singleClassDeclarationSyntax in classesDeclarationSyntax)
            {
                try
                {
                    var methodName = singleClassDeclarationSyntax.Identifier.Text;
                    var modelsReference = compilation.ExternalReferences
                        .SingleOrDefault(p => !String.IsNullOrWhiteSpace(p.Display) &&
                        p.Display!.EndsWith("FairPlayShop.Models.dll"));
                    var t = modelsReference!.GetType();
                    var metadata = compilation.GetTypeByMetadataName(t.Name);
                }
                catch (Exception)
                {
                }
            }

            StringBuilder sb = new();
            sb.AppendLine("partial class Program");
            sb.AppendLine("{");
            sb.AppendLine("static partial void ConfigureModelsLocalizers(IServiceProvider services)");
            sb.AppendLine("{");
            sb.AppendLine("}");
            sb.AppendLine("}");
            context.AddSource($"Program.g.cs",
                    SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}