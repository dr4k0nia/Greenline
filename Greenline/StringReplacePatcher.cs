using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Echo.DataFlow.Analysis;
using Echo.Platforms.AsmResolver;

// ReSharper disable once IdentifierTypo
namespace Greenline;

public static class StringReplacePatcher
{
    public static void Execute(MethodDefinition method)
    {
        var instructions = method.CilMethodBody!.Instructions;

        method.CilMethodBody.ConstructSymbolicFlowGraph(out var flowGraph);

        for (int i = 0; i < instructions.Count; i++)
        {
            var inst = instructions[i];

            if (inst.Operand is not IMethodDescriptor methodDescriptor)
                continue;

            if (!methodDescriptor.DeclaringType!.IsTypeOf("System", "String")
                || methodDescriptor.Name != "Replace"
                || !methodDescriptor.Signature!
                    .ParameterTypes.SequenceEqual(new[]
                    {
                        method.Module!.CorLibTypeFactory
                            .String,
                        method.Module.CorLibTypeFactory
                            .String
                    }))
                continue;

            var dependencies = flowGraph.Nodes[inst.Offset].GetOrderedDependencies().Select(n => n.Contents).ToList();

            string? target = dependencies.FirstOrDefault(ci => ci.OpCode == CilOpCodes.Ldstr)?.Operand?.ToString();

            if (target == null || dependencies[1].OpCode != CilOpCodes.Ldstr)
                continue;

            string pattern = dependencies[1].Operand!.ToString()!;

            string replacement = string.Empty;

            if (dependencies[2].OpCode == CilOpCodes.Ldstr)
                replacement = dependencies[2].Operand!.ToString()!;

            inst.ReplaceWith(CilOpCodes.Ldstr, target.Replace(pattern, replacement));

            var toRemove = dependencies.Where(ci => ci.Offset != inst.Offset);

            foreach (var element in toRemove)
            {
                element.ReplaceWithNop();
            }

            Console.WriteLine($"[Info]: Removed Replace in {method.Name} at offset {i}");
        }
    }
}
