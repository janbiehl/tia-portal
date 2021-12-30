using CommandLine;

namespace PlcSimAdvancedGateway;

// ReSharper disable once ClassNeverInstantiated.Global
public class CommandLineOptions
{
#pragma warning disable CS8618
    public static CommandLineOptions Options { get; internal set; }
#pragma warning restore CS8618
    
    [Option('v', "verbose", Default = false, HelpText = "Detailed log output will be provided")]
    public bool Verbose { get; set; }
    
    [Option('p', "port", Default = 9090, HelpText = "The port the grpc server is listening to")] 
    public int Port { get; set; }
    
}