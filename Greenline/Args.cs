using CommandLine;

namespace Greenline; 

public class Args {
    [Value(0, MetaName = "Module Path", HelpText = "Path to obfuscated module", Required = true)]
    public string ModulePath { get; set; }
    [Option('c', "config-only", HelpText = "Setting this flag will only dump the config.", Required = false)]
    public bool ConfigOnly { get; set; }
    [Option('s', "no-stack-calc", HelpText = "Setting this will disable \"Calculate Max Stack On Build\", which may fix an unhandled Exception", Required = false)]
    public bool CalculateStack { get; set; }
}
