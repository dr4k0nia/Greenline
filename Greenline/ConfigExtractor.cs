using System.Text;
using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;

namespace Greenline;

public static class ConfigExtractor
{
    public static void TryParseConfig(TypeDefinition type, out string? config)
    {
        config = null;

        if (!type.IsClass || type.IsNotPublic)
            return;

        if (type.GetStaticConstructor() == null || type.Fields.Count != 5)
            return;

        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!type.Fields.Any(f => f.Name == "IP"))
            return;

        var constructor = type.GetStaticConstructor();

        var instructions = constructor!.CilMethodBody?.Instructions;

        if (instructions == null)
            return;

        var elements =
            (from inst in instructions
                where inst.OpCode == CilOpCodes.Ldstr
                select inst.Operand!.ToString()!)
            .ToList();

        int version = instructions.FirstOrDefault(i => i.IsLdcI4())?.GetLdcI4Constant() ?? 0;

        string xorKey = elements[3];

        var builder = new StringBuilder();

        builder.Append("\n===[ Config ]===\n");
        builder.Append($"IP: {Decrypt(elements[0], xorKey)}\n");
        builder.Append($"ID: {Decrypt(elements[1], xorKey)}\n");
        builder.Append($"Message: {elements[2]}\n");
        builder.Append($"Xor Key: {elements[3]}\n");
        builder.Append($"Version: {version}\n");
        builder.Append("===[ Config ]===");

        config = builder.ToString();
    }

    private static string Decrypt(string input, string key)
    {
        string first = FromBase64(input);
        return FromBase64(Xor(first, key));
    }

    private static string FromBase64(string base64Str)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64Str));
    }

    // Token: 0x06000057 RID: 87 RVA: 0x000058F8 File Offset: 0x00003AF8
    private static string Xor(string input, string key)
    {
        var stringBuilder = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            int num = input[i] ^ key[i % key.Length];
            stringBuilder.Append(char.ConvertFromUtf32(num));
        }

        return stringBuilder.ToString();
    }
}
