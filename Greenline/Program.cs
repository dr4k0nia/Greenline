// See https://aka.ms/new-console-template for more information

using AsmResolver.DotNet;
using Greenline;

Console.WriteLine("Greenline | Redline Stealer String Unpacker by dr4k0nia\nhttps://github.com/dr4k0nia");

var module = ModuleDefinition.FromFile(args[0]);

bool configOnly = args.Length > 1 && args[1] == "--config-only" || args.Length > 2 && args[2] == "--config-only";
bool maxStack = args.Length > 1 && args[1] == "--max-stack" || args.Length > 2 && args[2] == "--max-stack";

string? parsedConfig = null;

foreach (var type in module.GetAllTypes())
{
    if (type.Methods.Count == 0)
        continue;

    if (parsedConfig == null)
        ConfigExtractor.TryParseConfig(type, out parsedConfig);

    if (!configOnly)
    {
        foreach (var method in type.Methods.Where(m => m.CilMethodBody != null)) {
            method.CilMethodBody!.ComputeMaxStackOnBuild = !maxStack;
            CharArrayPatcher.Execute(method);
            StringReplacePatcher.Execute(method);
        }
    }
}

Console.WriteLine(parsedConfig ?? "[Error]: Could not extract config");

module.Write(args[0].Insert(args[0].Length - 4, "_unpacked"));

Console.WriteLine("\nFinished unpacking");
Console.ReadKey();
