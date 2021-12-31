using System.Runtime.InteropServices;
using CommandLine;
using Grpc.Core;
using PlcSimAdvancedGateway;

// ensure the correct os, since PlcSimAdvanced from Siemens got a windows dependency,
// we will fail when not running on windows
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.WriteLine("This application will only run on windows");
    return (int) ExitCode.WrongOs;
}

// parse the command line args
Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsed(options => CommandLineOptions.Options = options);

Console.WriteLine("setting up the grpc server...");

// setup the grpc server
Server server = new()
{
    Services = {PlcSimAdvanced.Protos.PlcSimAdvancedService.BindService(new PlcSimAdvancedService())},
    Ports = {new("localhost", CommandLineOptions.Options.Port, ServerCredentials.Insecure)}
};

// start the grpc server
server.Start();

Console.WriteLine($"... grpc server started. listening on port: '{CommandLineOptions.Options.Port}'");

Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();

Console.WriteLine("application is shutting down...");
await server.ShutdownAsync();
Console.WriteLine("application stopped");

return 0;