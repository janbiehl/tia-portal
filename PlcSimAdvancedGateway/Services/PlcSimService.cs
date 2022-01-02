using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PlcSimAdvanced;
using PlcSimAdvanced.Protos;
using Siemens.Simatic.Simulation.Runtime;

namespace PlcSimAdvancedGateway.Services;

public class PlcSimService : PlcSimAdvanced.Protos.PlcSimService.PlcSimServiceBase
{
    private const string MessageHeader = "message";
    private const string ExceptionHeader = "exception";
    
    private readonly ILogger<PlcSimService> _logger;
    private readonly PlcInstanceManager _plcManager;
    private readonly RuntimeManagerGateway _gateway = new ();

    public static ServerServiceDefinition BindService(PlcSimAdvanced.Protos.PlcSimService.PlcSimServiceBase? serviceBase)
    {
        return PlcSimAdvanced.Protos.PlcSimService.BindService(serviceBase);
    }
    
#region CTOR

    public PlcSimService(ILogger<PlcSimService> logger, PlcInstanceManager plcManager)
    {
        _logger = logger;
        _plcManager = plcManager;
    }

#endregion

    public override async Task<GetVersionResponse> GetVersion(GetVersionRequest request, ServerCallContext context)
    {
        _logger.LogTrace("GetVersion invoked via GRPC");

#if DEBUG_MAC
        await context.WriteResponseHeadersAsync(
            new() {
                new(
                    "Simulation", 
                    "Caution the server will provide simulated data")
            });
#endif
        
        GetVersionResponse response = new()
        {
            MajorVersion = _gateway.Version.Major,
            MinorVersion = _gateway.Version.Minor,
            Valid = _gateway.Version.Valid
        };

        return response;
    }

    public override async Task<ShutdownResponse> Shutdown(ShutdownRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(Shutdown)} invoked via GRPC");

#if DEBUG_MAC
        await context.WriteResponseHeadersAsync(
            new() {
                new(
                    "Simulation", 
                    "Caution the server will provide simulated data")
            });
#endif
        
        try
        {
            _gateway.Shutdown();
        }
        catch (SimulationRuntimeException e)
        {
            const string message = "There was an error while shutting down the plc instance";

            _logger.LogError(e, message);
            
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, message)});
            
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<GetInstancesResponse> GetRegisteredInstances(GetInstancesRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(GetRegisteredInstances)} invoked via GRPC");

#if DEBUG_MAC
        await context.WriteResponseHeadersAsync(
            new() {
                new(
                    "Simulation", 
                    "Caution the server will provide simulated data")
            });
#endif
        
        try
        {
            var instances = _gateway.RegisteredInstances;
            
            GetInstancesResponse response = new();

            foreach (var instance in instances)
            {
                response.Instances.Add(new PlcSimAdvanced.Protos.InstanceInfo {Name = instance.Name, Id = instance.ID});
            }

            return response;
        }
        catch (SimulationRuntimeException e)
        {
            const string message = "There was an error while getting the registered instances";
            
            _logger.LogError(e, message);
            
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, message)});

            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<RegisterInstanceResponse> RegisterInstance(RegisterInstanceRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(RegisterInstance)} invoked via GRPC");

#if DEBUG_MAC
        await context.WriteResponseHeadersAsync(
            new() {
                new(
                    "Simulation", 
                    "Caution the server will provide simulated data")
            });
#endif
        
        // Validate arguments
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            const string message = $"The '{nameof(request.Name)}' is not allowed to be null, empty or only whitespace";
            
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, message)});
            context.Status = new Status(StatusCode.InvalidArgument, message);
            
            return new RegisterInstanceResponse();
        }

        try
        {
            SInstanceInfo[] registeredInstances = _gateway.RegisteredInstances;
            
            // Check if there is already an instance with that name in the runtime manager 
            foreach (var registeredInstance in registeredInstances)
            {
                if (registeredInstance.Name != request.Name)
                    continue;

                var instance = _gateway.GetInstance(registeredInstance.ID);

                if (instance is null) 
                    continue;

                // store the instance in our memory
                var result = await _plcManager.AddPlcInstanceAsync(instance, context.CancellationToken);

                if (result is false)
                {
                    await context.WriteResponseHeadersAsync(new()
                        {new(MessageHeader, "There was an error registering the instance. It is already known")});
                }

                // sent the result back to the caller
                RegisterInstanceResponse response = new()
                {
                    Instance = new(instance)
                };

                return response;
            }

            var newInstance = _gateway.RegisterInstance(request.Name);

            if (newInstance is not null)
            {
                await context.WriteResponseHeadersAsync(new()
                    {new(MessageHeader, "Successfully registered the instance")});

                
                // store the instance in our memory
                var result = await _plcManager.AddPlcInstanceAsync(newInstance, context.CancellationToken);

                if (result is false)
                {
                    await context.WriteResponseHeadersAsync(new()
                        {new(MessageHeader, "There was an error registering the instance. It is already known")});
                }
                
                // sent the result back to the caller
                RegisterInstanceResponse response = new()
                {
                    Instance = new(newInstance)
                };

            }
        }
        catch (SimulationRuntimeException e)
        {
            const string message = "There was an error while registering the instance";

            _logger.LogError(e, message);
            
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, message)});

            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<UnregisterInstanceResponse> UnregisterInstance(UnregisterInstanceRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(UnregisterInstance)} invoked via GRPC");

#if DEBUG_MAC
        await context.WriteResponseHeadersAsync(
            new() {
                new(
                    "Simulation", 
                    "Caution the server will provide simulated data")
            });
#endif
        
        var plcInstance = await _plcManager.GetPlcInstanceAsync(request.Instance, context.CancellationToken);

        if (plcInstance is null)
        {
            const string message = $"There is no plc instance for the requested name or id";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }

        try
        {
            plcInstance.Unregister();
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while cleaning up the controllers storage (virtual memory card)";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    
    public override async Task<GetPlcInformationResponse> GetPlcInformation(GetPlcInformationRequest request, ServerCallContext context)
    {        
        _logger.LogTrace($"{nameof(GetPlcInformation)} invoked via GRPC");

#if DEBUG_MAC
        await context.WriteResponseHeadersAsync(
            new() {
                new(
                    "Simulation", 
                    "Caution the server will provide simulated data")
            });
#endif
        
        PlcInstance? plcInstance = null;
        
        if (plcInstance is null)
        {
            const string message = $"There is no plc instance for the requested name or id";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);
            
            return new();
        }

        try
        {
            GetPlcInformationResponse response = new()
            {
                CpuType = plcInstance.CpuType.ToString(),
                CommunicationInterface = plcInstance.CommunicationInterface.ToString(),
                ControllerName = plcInstance.ControllerName,
                ControllerShortDesignation = plcInstance.ControllerShortDesignation,
                ControllerIp = PlcSimAdvanced.Utils.GetIp(plcInstance.ControllerIp),
                // TODO: Add the ip suites from siemens
                ControllerIpSuites = { },
                StoragePath = plcInstance.StoragePath,
                OperatingState = plcInstance.OperatingState.ToString(),
                ControllerTime = Timestamp.FromDateTime(plcInstance.Time),
                ControllerTimescale = plcInstance.TimeScale,
                OperatingMode = plcInstance.OperatingMode.ToString(),
                IsSendSyncEventInDefaultModeEnabled = plcInstance.IsSendSyncEventInDefaultModeEnabled,
                OverwrittenMinimalCycleTime = plcInstance.OverwrittenMinimalCycleTime
            };

            return response;
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error fetching the information from the controller";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }
}