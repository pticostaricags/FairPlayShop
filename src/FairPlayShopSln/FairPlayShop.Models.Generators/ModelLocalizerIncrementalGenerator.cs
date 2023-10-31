using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace FairPlaySocial.Services.Generators
{
    [Generator]
    public class ModelLocalizerIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
#endif
            // Do a simple filter for enums
            IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
                context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
                .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

            // Combine the selected interfaces with the `Compilation`
            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)>
                compilationAndClasses
                = context.CompilationProvider.Combine(classDeclarations.Collect());

            // Generate the source using the compilation and classes
            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext generatorSyntaxContext)
        {
            var classDeclarationSyntax = generatorSyntaxContext.Node as ClassDeclarationSyntax;
            return classDeclarationSyntax!;
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
            {
                foreach (var singleAttributeList in classDeclarationSyntax.AttributeLists)
                {
                    foreach (var singleAttribute in singleAttributeList.Attributes)
                    {
                        var identifierNameSyntax = (singleAttribute.Name) as GenericNameSyntax;
                        string identifierText = identifierNameSyntax!.Identifier.Text;
                        if (identifierText == "LocalizerOfT")
                            return true;
                    }
                }
            }
            return false;
        }

        static void Execute(Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classesDeclarationSyntax, SourceProductionContext context)
        {
            string assemblyName = compilation.AssemblyName!;
            string[] splittedAssemblyName = assemblyName.Split('.');
            string assemblyNameFirstPart = splittedAssemblyName[0];
            foreach (var singleClassDeclarationSyntax in classesDeclarationSyntax)
            {
                var serviceName = singleClassDeclarationSyntax.Identifier.Text;
                foreach (var singleAttributeList in singleClassDeclarationSyntax.AttributeLists)
                {
                    foreach (var singleAttribute in singleAttributeList.Attributes)
                    {
                        var identifierNameSyntax = (singleAttribute.Name) as GenericNameSyntax;
                        string identifierText = identifierNameSyntax!.Identifier.Text;
                        if (identifierText == "LocalizerOfT")
                        {
                            var typeArgument = identifierNameSyntax!.TypeArgumentList;
                            var typeArgumentIdentifier = typeArgument!.Arguments[0] as IdentifierNameSyntax;
                            var typeArgumentName = typeArgumentIdentifier!.Identifier!.ValueText;
                            var newClassName = $"{typeArgumentName}Localizer";
                            var indexOfLastModel = typeArgumentName.LastIndexOf("Model");
                            var entityName = typeArgumentName.Substring(6, indexOfLastModel - 6);
                            string namespaceValue = $"FairPlayShop.Models.{entityName}";
                            var fullyQualifiedMetadataName = $"{namespaceValue}.{typeArgumentName}";
                            var typedSymbol = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
                            var properties = typedSymbol.GetMembers().Where(p => p.Kind == SymbolKind.Property);
                            StringBuilder stringBuilder = new();
                            stringBuilder.AppendLine("using Microsoft.Extensions.Localization;");
                            stringBuilder.AppendLine($"namespace {namespaceValue};");
                            stringBuilder.AppendLine($"public partial class {newClassName}");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"public static IStringLocalizer<{newClassName}> Localizer {{ get; set; }}");
                            foreach (var singleProperty in properties)
                            {
                                var propertyAttributes = singleProperty.GetAttributes();
                                foreach (var singlePropertyAttribute in propertyAttributes)
                                {
                                    var singlePropertyMetadataName = singlePropertyAttribute!.AttributeClass!.MetadataName;
                                    switch (singlePropertyMetadataName)
                                    {
                                        case "RequiredAttribute":
                                            stringBuilder.AppendLine($"public static string {singleProperty.Name}_Required => Localizer[\"{singleProperty.Name}_Required\"];");
                                            break;
                                        case "StringLengthAttribute":
                                            stringBuilder.AppendLine($"public static string {singleProperty.Name}_StringLength => Localizer[\"{singleProperty.Name}_StringLength\"];");
                                            break;
                                    }
                                }
                            }
                            stringBuilder.AppendLine("}");
                            context.AddSource($"{newClassName}.g.cs",
                        SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
                        }
                    }
                }
            }
        }
    }
}