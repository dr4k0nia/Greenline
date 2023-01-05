// See https://aka.ms/new-console-template for more information

using AsmResolver.DotNet;
using CommandLine;
using Greenline;

Console.WriteLine("Greenline | Redline Stealer String Unpacker by dr4k0nia\nhttps://github.com/dr4k0nia");
Args pArgs = Parser.Default.ParseArguments<Args>(args).WithParsed(o => {}).Value;

if (pArgs == null) {
    Console.ReadKey();
    return;
}

var module = ModuleDefinition.FromFile(pArgs.ModulePath);

string? parsedConfig = null;

foreach (var type in module.GetAllTypes())
{
    if (type.Methods.Count == 0)
        continue;

    if (parsedConfig == null)
        ConfigExtractor.TryParseConfig(type, out parsedConfig);

    if (!pArgs.ConfigOnly)
    {
        foreach (var method in type.Methods.Where(m => m.CilMethodBody != null)) {
            method.CilMethodBody!.ComputeMaxStackOnBuild = !pArgs.CalculateStack;
            CharArrayPatcher.Execute(method);
            StringReplacePatcher.Execute(method);
        }
    }
}

Console.WriteLine(parsedConfig ?? "[Error]: Could not extract config");

module.Write(args[0].Insert(args[0].Length - 4, "_unpacked"));

Console.WriteLine("\nFinished unpacking");
Console.ReadKey();
