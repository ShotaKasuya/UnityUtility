using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SerializableGenerator;

[Generator(LanguageNames.CSharp)]
public class ExampleGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assemblyName = context.CompilationProvider.Select(static (compilation, _) => compilation.AssemblyName);
        context.RegisterSourceOutput(assemblyName, (spc, currentAssemblyName) =>
        {
            if (currentAssemblyName == "Assembly-CSharp")
            {
                spc.AddSource("ExampleSourceGenerator.g.cs", GenerateSource());
            }
        });
    }

    private SourceText GenerateSource()
    {
        using var sourceStream = new StringWriter();
        using var codeWriter = new IndentedTextWriter(sourceStream);

        codeWriter.WriteLine("using System;");
        codeWriter.WriteLine("namespace ExampleSourceGenerated {");
        codeWriter.Indent++;

        codeWriter.WriteLine("public static class ExampleSourceGenerated {");
        codeWriter.Indent++;

        codeWriter.WriteLine("public static string GetTestText()");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;

        codeWriter.WriteLine("return \"This is from incremental generator - Generated at build time\";");

        codeWriter.Indent--;
        codeWriter.WriteLine("}");

        codeWriter.Indent--;
        codeWriter.WriteLine("}");

        codeWriter.Indent--;
        codeWriter.WriteLine("}");

        return SourceText.From(sourceStream.ToString(), Encoding.UTF8);
    }
}