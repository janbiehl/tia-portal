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
                    Instance = new()
                    {
                        Id = _plcInstances[instanceInfo].Id,
                        Name = _plcInstances[instanceInfo].Name
                    }
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
                
                RegisterInstanceResponse response = new()
                {
                    Instance = new()
                    {
                        Id = instance.Id,
                        Name = instance.Name
                    }
                };

                return response;
            }

            var newInstance = _gateway.RegisterInstance(request.Name);

            if (newInstance is not null)
            {
                await context.WriteResponseHeadersAsync(new()
                    {new(MessageHeader, "Successfully registered the instance")});

                RegisterInstanceResponse response = new()
                {
                    Instance = new()
                    {
                        Id = newInstance.Id,
                        Name = newInstance.Name
                    }
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
}