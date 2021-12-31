using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PlcSimAdvanced;
using PlcSimAdvanced.Protos;
using Siemens.Simatic.Simulation.Runtime;

namespace PlcSimAdvancedGateway;

public class PlcSimAdvancedService : PlcSimAdvanced.Protos.PlcSimAdvancedService.PlcSimAdvancedServiceBase
{
    private const string MessageHeader = "message";
    private const string ExceptionHeader = "exception";
    
    private readonly RuntimeManagerGateway _gateway = new ();
    private readonly Dictionary<InstanceInfo, PlcInstance> _plcInstances = new();

    public override Task<GetVersionResponse> GetVersion(GetVersionRequest request, ServerCallContext context)
    {
        RuntimeManagerGateway gateway = new ();

        GetVersionResponse response = new()
        {
            MajorVersion = gateway.Version.Major,
            MinorVersion = gateway.Version.Minor,
            Valid = gateway.Version.Valid
        };

        return Task.FromResult(response);
    }

    public override async Task<ShutdownResponse> Shutdown(ShutdownRequest request, ServerCallContext context)
    {
        try
        {
            _gateway.Shutdown();
        }
        catch (SimulationRuntimeException e)
        {
            Console.WriteLine(e);
            
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, "There was an error while registering the instance")});

            throw;
        }

        return new ShutdownResponse();
    }

    public override async Task<GetInstancesResponse> GetRegisteredInstances(GetInstancesRequest request, ServerCallContext context)
    {
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
            Console.WriteLine(e);
            
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, "There was an error while getting the registered instances")});

            throw;
        }
    }

    public override async Task<RegisterInstanceResponse> RegisterInstance(RegisterInstanceRequest request, ServerCallContext context)
    {
        // Validate arguments
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, "The 'name' is not allowed to be null, empty or only whitespace")});
            context.Status = new Status(StatusCode.InvalidArgument,
                "The 'name' is not allowed to be null, empty or only whitespace");
            
            return new RegisterInstanceResponse();
        }

        try
        {
            SInstanceInfo[] registeredInstances = _gateway.RegisteredInstances;

            // Check if there is a registered instance in our memory
            InstanceInfo instanceInfo = new() {Name = request.Name};

            if (_plcInstances.ContainsKey(instanceInfo))
            {
                RegisterInstanceResponse response = new()
                {
                    Instance = new(_plcInstances[instanceInfo])
                };

                return response;
            }
            
            // Check if there is already an instance with that name in the runtime manager 
            foreach (var registeredInstance in registeredInstances)
            {
                if (registeredInstance.Name != request.Name)
                    continue;

                var instance = _gateway.GetInstance(registeredInstance.ID);

                if (instance is null) 
                    continue;

                // store the instance in our memory
                _plcInstances.Add(new(instance), instance);
                
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
                _plcInstances.Add(new(newInstance), newInstance);
                
                // sent the result back to the caller
                RegisterInstanceResponse response = new()
                {
                    Instance = new(newInstance)
                };

            }
        }
        catch (SimulationRuntimeException e)
        {
            Console.WriteLine(e);
       
            await context.WriteResponseHeadersAsync(new()
                {new(MessageHeader, "There was an error while registering the instance")});

            throw;
        }

        return new RegisterInstanceResponse();
    }

    public override 
        Task<GetPlcInformationResponse> GetPlcInformation(GetPlcInformationRequest request, ServerCallContext context)
    {
        PlcInstance? plcInstance = null;

        switch (request.Instance.DataOneofCase)
        {
            case InstanceInfoRequest.DataOneofOneofCase.Id:
            {
                if (request.Instance.Id is 0)
                {
                    context.Status = new Status(StatusCode.InvalidArgument, "The id may not be 0");
                    return Task.FromResult<GetPlcInformationResponse>(new());
                }
                
                foreach (var instance in _plcInstances)
                {
                    if (instance.Key.Id == request.Instance.Id)
                        plcInstance = instance.Value;
                }
                
                break;
            }
            case InstanceInfoRequest.DataOneofOneofCase.Name:
            {
                if (string.IsNullOrWhiteSpace(request.Instance.Name))
                {
                    context.Status = new Status(StatusCode.InvalidArgument,
                        "The name may not be null, empty or only whitespace");

                    return Task.FromResult<GetPlcInformationResponse>(new());
                }

                foreach (var instance in _plcInstances)
                {
                    if (instance.Key.Name == request.Instance.Name)
                        plcInstance = instance.Value;
                }
                
                break;
            }
            default:
            {
                context.Status = new(StatusCode.InvalidArgument, "Id or Name must be used");
                return Task.FromResult<GetPlcInformationResponse>(new());
            }
        }
        
        if (plcInstance is null)
        {
            // TODO: Error here, no instance found
            return Task.FromResult(new GetPlcInformationResponse());
        }

        GetPlcInformationResponse response = new()
        {
            CpuType = plcInstance.CpuType.ToString(),
            CommunicationInterface = plcInstance.CommunicationInterface.ToString(),
            ControllerName = plcInstance.ControllerName,
            ControllerShortDesignation = plcInstance.ControllerShortDesignation,
            ControllerIp = Utils.GetIp(plcInstance.ControllerIp),
            // TODO: Add the ip suites from siemens
            ControllerIpSuites = {  },
            StoragePath = plcInstance.StoragePath,
            OperatingState = plcInstance.OperatingState.ToString(),
            ControllerTime = Timestamp.FromDateTime(plcInstance.Time),
            ControllerTimescale = plcInstance.TimeScale,
            OperatingMode = plcInstance.OperatingMode.ToString(),
            IsSendSyncEventInDefaultModeEnabled = plcInstance.IsSendSyncEventInDefaultModeEnabled,
            OverwrittenMinimalCycleTime = plcInstance.OverwrittenMinimalCycleTime
        };

        return Task.FromResult(response);
    }
}