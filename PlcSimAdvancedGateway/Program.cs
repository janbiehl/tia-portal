using System.Runtime.InteropServices;
using CommandLine;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlcSimAdvanced.Protos;
using PlcSimAdvancedGateway;
using PlcSimAdvancedGateway.Services;
using PlcSimService = PlcSimAdvancedGateway.Services.PlcSimService;

// set up the dependency injection
await using var serviceProvider = new ServiceCollection()
    .AddLogging((builder) =>
    {
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
            .AddConsole();
    })
    .AddSingleton<PlcInstanceManager>()
    .AddSingleton<PlcSimService>()
    .AddSingleton<ControllerService>()
    .BuildServiceProvider();

// get the logger
ILogger logger = (serviceProvider.GetService<ILoggerFactory>() ?? throw new InvalidOperationException("There is no logger"))
    .CreateLogger<Program>();

// ensure the correct os, since PlcSimAdvanced from Siemens got a windows dependency,
// we will fail when not running on windows
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
#if !DEBUG_MAC
    logger.LogError("this application will only run on windows");
    return (int) ExitCode.WrongOs;
#else
    logger.LogWarning("application is running on a unsupported os, only basic testing is enabled");
#endif
}

// parse the command line args
Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsed(options => CommandLineOptions.Options = options);

logger.LogInformation("setting up the grpc server");

var reflectionService = new ReflectionServiceImpl(
    PlcSimAdvanced.Protos.PlcSimService.Descriptor,
    VirtualControllerService.Descriptor, 
    ServerReflection.Descriptor);

// setup the grpc server
Server server = new()
{
    Services = 
    {
        ServerReflection.BindService(reflectionService),
        PlcSimService.BindService(serviceProvider.GetService<PlcSimService>()),
        VirtualControllerService.BindService(serviceProvider.GetService<ControllerService>()),
    },
    Ports = {new("localhost", CommandLineOptions.Options.Port, ServerCredentials.Insecure)}
};

// start the grpc server
server.Start();

logger.LogInformation($"grpc server started, it is listening on port: '{CommandLineOptions.Options.Port}'");

logger.LogInformation("Press any key to stop the application...");
Console.ReadKey();

logger.LogInformation("application is shutting down...");
await server.ShutdownAsync();
logger.LogInformation("application stopped");

return 0;