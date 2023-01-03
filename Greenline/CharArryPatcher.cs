using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Echo.DataFlow.Analysis;
using Echo.Platforms.AsmResolver;

// ReSharper disable once IdentifierTypo
namespace Greenline;

public static class CharArrayPatcher
{
    public static void Execute(MethodDefinition method)
    {
        var instructions = method.CilMethodBody!.Instructions;

        method.CilMethodBody.ConstructSymbolicFlowGraph(out var flowGraph);

        for (int i = 0; i < instructions.Count; i++)
        {
            var inst = instructions[i];

            if (i <= 0)
                continue;

            if (inst.OpCode != CilOpCodes.Newobj || instructions[i - 1].OpCode != CilOpCodes.Call)
                continue;

            if (inst.Operand is not IMethodDescriptor constructorDef)
                continue;

            // We are searching for System.String::.ctor(char[])
            if (!constructorDef!.DeclaringType!.IsTypeOf("System", "String")
                && constructorDef.Signature!.ParameterTypes[0] is not SzArrayTypeSignature
                {
                    ElementType: ElementType.Char
                })
                continue;

            var dependencies = flowGraph.Nodes[instructions[i - 1].Offset].GetOrderedDependencies()
                .Select(n => n.Contents)
                .ToList();

            if (dependencies.FirstOrDefault(ci => ci.OpCode == CilOpCodes.Ldtoken)?.Operand is not FieldDefinition
                {
                    FieldRva: IReadableSegment segment
                })
                continue;

            char[] array = new char[dependencies.First(ci => ci.IsLdcI4()).GetLdcI4Constant()];

            var reader = segment.CreateReader();
            for (int index = 0; index < array.Length; index++)
            {
                array[index] = (char) reader.ReadInt16();
            }

            foreach (var element in dependencies)
            {
                element.ReplaceWithNop();
            }

            inst.ReplaceWith(CilOpCodes.Ldstr, new string(array));

            Console.WriteLine($"[Info]: Fixed array string in {method.Name} at offset {i}");
        }

        instructions.OptimizeMacros();
    }
}
