// See https://aka.ms/new-console-template for more information

using AsmResolver.DotNet;
using Greenline;

Console.WriteLine("Greenline | Redline Stealer String Unpacker by dr4k0nia\nhttps://github.com/dr4k0nia");

var module = ModuleDefinition.FromFile(args[0]);

bool configOnly = args.Length == 2 && args[1] == "--config-only";

string? parsedConfig = null;

foreach (var type in module.GetAllTypes())
{
    if (type.Methods.Count == 0)
        continue;

    ConfigExtractor.TryParseConfig(type, out string? config);

    if (config != null)
        parsedConfig = config;

    if (!configOnly)
    {
        foreach (var method in type.Methods.Where(m => m.CilMethodBody != null))
        {
            CharArrayPatcher.Execute(method);
            StringReplacePatcher.Execute(method);
        }
    }
}

Console.WriteLine(parsedConfig ?? "[Error]: Could not extract config");

module.Write(args[0].Insert(args[0].Length - 4, "_unpacked"));

Console.WriteLine("\nFinished unpacking");
Console.ReadKey();
