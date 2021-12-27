using System.Runtime.InteropServices;
using PlcSimAdvanced;
using PlcSimAdvancedGateway;
using Siemens.Simatic.Simulation.Runtime;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.WriteLine("This application will only run on windows");
    return (int) ExitCode.WrongOs;
}

RuntimeManagerGateway gateway = new();

var controllerInstances = gateway.GetRegisteredInstances();

foreach (var instance in controllerInstances)
{
    Console.WriteLine($"Instance '{instance.Name}', '{instance.ID}'");
}

var selectedInstance = gateway.GetInstance(controllerInstances[0].ID);

if (selectedInstance is null)
{
    Console.WriteLine("There is no instance to connect to");
    return (int) ExitCode.Unknown;
}

Task.Run(async () =>
{
    bool value = true;
    
    while (true)
    {
        selectedInstance.WriteInputBit(0, 0, value);
        selectedInstance.WriteInputBit(0, 1, value);
        selectedInstance.WriteInputBit(0, 2, value);
        selectedInstance.WriteInputBit(0, 3, value);
        selectedInstance.WriteInputBit(0, 4, value);
        selectedInstance.WriteInputBit(0, 5, value);
        selectedInstance.WriteInputBit(0, 6, value);
        selectedInstance.WriteInputBit(0, 7, value);

        selectedInstance.WriteInputByte(1, value ? (byte) 255 : (byte) 0);
        
        value = !value;
        await Task.Delay(2000);
    }
}, CancellationToken.None);
