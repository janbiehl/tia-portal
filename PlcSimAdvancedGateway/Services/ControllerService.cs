using System.Text;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PlcSimAdvanced.Protos;
using Siemens.Simatic.Simulation.Runtime;

namespace PlcSimAdvancedGateway.Services;

public class ControllerService : VirtualControllerService.VirtualControllerServiceBase
{
    private const string MessageHeader = "message";
    private const string ExceptionHeader = "exception";
    
    private readonly ILogger<ControllerService> _logger;
    private readonly PlcInstanceManager _plcManager;

    public ControllerService(ILogger<ControllerService> logger, PlcInstanceManager plcManager)
    {
        _logger = logger;
        _plcManager = plcManager;
    }

    public override async Task<ControllerRunResponse> ControllerRun(ControllerRunRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ControllerRun)} invoked via GRPC");

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
            if (request.Timeout == 0)
            {
                plcInstance.Run();
            }
            else
            {
                plcInstance.Run(request.Timeout);
            }
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance), 
                Running = plcInstance.OperatingState == EOperatingState.Run
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error starting the controller";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }
        
        return new ();
    }

    public override async Task<ControllerStopResponse> ControllerStop(ControllerStopRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ControllerStop)} invoked via GRPC");

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
            if (request.Timeout == 0)
            {
                plcInstance.Stop();
            }
            else
            {
                plcInstance.Run(request.Timeout);
            }

            return new()
            {
                Instance = new InstanceInfo(plcInstance), 
                Stopped = plcInstance.OperatingState == EOperatingState.Stop
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error stopping the controller";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<ControllerStopResponse> ControllerMemoryReset(ControllerStopRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ControllerMemoryReset)} invoked via GRPC");

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
            if (request.Timeout == 0)
            {
                plcInstance.MemoryRest();
            }
            else
            {
                plcInstance.MemoryRest(request.Timeout);
            }
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Stopped = plcInstance.OperatingState == EOperatingState.Stop
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error resetting the controller";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<ControllerPowerOffResponse> ControllerPowerOff(ControllerPowerOffRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ControllerPowerOff)} invoked via GRPC");

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
            if (request.Timeout == 0)
            {
                plcInstance.PowerOff();
            }
            else
            {
                plcInstance.PowerOff(request.Timeout);
            }

            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                // TODO: Do we got a signal here for it
                PoweredOff = true
            };
        }        
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error powering off the controller";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<ControllerPowerOnResponse> ControllerPowerOn(ControllerPowerOnRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ControllerPowerOn)} invoked via GRPC");

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
            if (request.Timeout == 0)
            {
                plcInstance.PowerOn();
            }
            else
            {
                plcInstance.PowerOn(request.Timeout);
            }

            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                // TODO: Do we got a signal here for it
                PoweredOn = true
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error powering on the controller";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<CleanupControllerStorageResponse> CleanupControllerStorage(CleanupControllerStorageRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(CleanupControllerStorage)} invoked via GRPC");

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
            plcInstance.CleanupStoragePath();

            return new()
            {
                Instance = new InstanceInfo(plcInstance)
            };
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
    
    public override async Task<CreateConfigFileResponse> CreateConfigFile(CreateConfigFileRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(CreateConfigFile)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.FilePath))
        { 
            const string message = $"The {nameof(request.FilePath)} is not allowed to be null, empty or whitespace only";
           
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }
        
        /*if (File.Exists(request.FilePath))
        {
            // The file does already exist
            if (request.OverwriteFile)
            {
                try
                {
                    File.Delete(request.FilePath);
                }
                catch (Exception e)
                {
                    const string message = "There was an error deleting the requested file";
                    
                    _logger.LogError(e, message);
                
                    await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
                    await context.WriteResponseHeadersAsync(new() {new(ExceptionHeader, e.ToString())});

                    context.Status = new Status(StatusCode.InvalidArgument, message);

                    return new();
                }
            }
            else
            {
                string message = $"The file '{request.FilePath}' is already existing. But overwriting is not allowed";
                
                await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

                context.Status = new Status(StatusCode.InvalidArgument, message);

                return new();
            }
        }*/
        
        try
        {
            var overwritten = plcInstance.CreateConfigurationFile(request.FilePath, request.OverwriteFile);

            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                FileOverwritten = overwritten,
                FilePath = request.FilePath
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while creating the configuration file";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<CreateStorageArchiveResponse> CreateStorageArchive(CreateStorageArchiveRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(CreateStorageArchive)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.ArchivePath))
        { 
            const string message = $"The {nameof(request.ArchivePath)} is not allowed to be null, empty or whitespace only";
           
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }

        bool overwritten = false;
        
        if (request.ArchiveOverwrite && Directory.Exists(request.ArchivePath))
        {
            Directory.Delete(request.ArchivePath, true);
            overwritten = true;
        }

        try
        {
            plcInstance.ArchiveStorage(request.ArchivePath);

            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                ArchiveCreated = true,
                ArchiveOverwritten = overwritten
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while creating the storage archive";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<RetrieveStorageArchiveResponse> RetrieveStorageArchive(RetrieveStorageArchiveRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(RetrieveStorageArchive)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.ArchivePath))
        { 
            const string message = $"The {nameof(request.ArchivePath)} is not allowed to be null, empty or whitespace only";
           
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }

        if (!Directory.Exists(request.ArchivePath))
        {
            const string message = $"There is no archive at the following path '{nameof(request.ArchivePath)}'";
           
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }
        
        try
        {
            plcInstance.RetrieveStorage(request.ArchivePath);

            return new()
            {
                Instance = new InstanceInfo(plcInstance)
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while restoring the archive";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<GetTagInfoResponse> GetTagInfo(GetTagInfoRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(GetTagInfo)} invoked via GRPC");

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

        if (request.Count == 0)
        {
            const string message = $"The {nameof(request.Count)} may not be 0";
           
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }

        try
        {
            var tagInfos = plcInstance.TagInfos;
            GetTagInfoResponse response = new()
            {
                Instance = new(plcInstance)
            };
            
            
            for (var i = request.StartIndex; i < (request.StartIndex + request.Count); i++)
            {
                var tagInfo = tagInfos[i];
                
                response.PlcTags.Add(new PlcTag
                {
                    Area = Utils.GetMemoryArea(tagInfo.Area),
                    Bit = tagInfo.Bit,
                    Datatype = Utils.GetDatatype(tagInfo.DataType),
                    Index = tagInfo.Index,
                    Name = tagInfo.Name,
                    Offset = tagInfo.Offset,
                    Size = tagInfo.Size,
                    PrimitiveDataType = Utils.GetPrimitiveDatatype(tagInfo.PrimitiveDataType)
                });
            }

            return response;
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while reading tag info";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new();
    }

    public override async Task<SetStoragePathResponse> SetStoragePath(SetStoragePathRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(SetStoragePath)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.StoragePath))
        { 
            const string message = $"The {nameof(request.StoragePath)} is not allowed to be null, empty or whitespace only";
           
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new();
        }

        try
        {
            plcInstance.StoragePath = request.StoragePath;

            return new()
            {
                Instance = new(plcInstance),
                StoragePath = plcInstance.StoragePath
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while setting the storage path";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }
        
        return new ();
    }

    public override async Task<UpdateTagListResponse> UpdateTagList(UpdateTagListRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(UpdateTagList)} invoked via GRPC");

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
            if (request.DataBlockFilter is null || request.DataBlockFilter.Count == 0)
            {
                var filter = Utils.GetTagListDetails(request.Filter);
                plcInstance.UpdateTagList(filter, request.HmiVisibleOnly);
            }
            else
            {
                plcInstance.UpdateTagList(request.DataBlockFilter.ToArray(), ETagListDetails.DB, request.HmiVisibleOnly);
            }

            return new()
            {
                Instance = new(plcInstance)
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while updating the tag list";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }
        
        return new ();
    }

    public override async Task<GetTagListStatusResponse> GetTagListStatus(GetTagListStatusRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(GetTagListStatus)} invoked via GRPC");

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
            plcInstance.GetTagListStatus(out var details, out var hmiVisibleOnly);

            return new()
            {
                Instance = new(plcInstance),
                Filter = Utils.GetTagListDetails(details),
                HmiVisibleOnly = hmiVisibleOnly
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error while getting the tag lists status";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }
        
        return new ();
    }

#region ReadValues
    
    public override async Task<ReadCharResponse> ReadChar(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadChar)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ReadCharResponse();
        }

        try
        {        
            var value = plcInstance.ReadChar(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = ByteString.CopyFrom((byte) value)
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ReadCharResponse();
    }

    public override async Task<ReadBooleanResponse> ReadBoolean(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadBoolean)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadBool(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadDoubleResponse> ReadDouble(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadDouble)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadDouble(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadFloatResponse> ReadFloat(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadFloat)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadFloat(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }
    
    public override async Task<ReadInt8Response> ReadInt8(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadInt8)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadInt8(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadInt16Response> ReadInt16(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadInt16)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadInt16(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadInt32Response> ReadInt32(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadInt32)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadInt32(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadInt64Response> ReadInt64(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadInt64)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadInt64(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadUInt8Response> ReadUInt8(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadUInt8)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadUInt8(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadUInt16Response> ReadUInt16(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadUInt16)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadUInt16(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadUInt32Response> ReadUInt32(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadUInt32)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadUInt32(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadUInt64Response> ReadUInt64(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadUInt64)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadUInt64(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = value
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

    public override async Task<ReadWCharResponse> ReadWChar(ReadTagRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(ReadWChar)} invoked via GRPC");

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

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            var value = plcInstance.ReadWChar(request.TagName);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                Value = ByteString.CopyFrom(Encoding.UTF8.GetBytes(new []{value}))
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error reading the value from the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();
    }

#endregion

#region WriteValues

    public override async Task<WriteTagResponse> WriteBoolean(WriteBooleanRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteBoolean)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {        
            plcInstance.WriteBool(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteChar(WriteCharRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteChar)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteChar(request.TagName, (sbyte) request.Value[0]);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteDouble(WriteDoubleRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteDouble)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteDouble(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteFloat(WriteFloatRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteFloat)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteFloat(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteInt8(WriteInt8Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteInt8)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteInt8(request.TagName, (sbyte) request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteInt16(WriteInt16Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteInt16)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteInt16(request.TagName, (short) request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteInt32(WriteInt32Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteInt32)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteInt32(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteInt64(WriteInt64Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteInt64)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteInt64(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteUInt8(WriteUInt8Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteUInt8)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteUInt8(request.TagName, (byte) request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteUInt16(WriteUInt16Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteUInt16)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteUInt16(request.TagName, (ushort) request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteUInt32(WriteUInt32Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteUInt32)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteUInt32(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteUInt64(WriteUInt64Request request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteUInt64)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            plcInstance.WriteUInt64(request.TagName, request.Value);
            
            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

    public override async Task<WriteTagResponse> WriteWChar(WriteWCharRequest request, ServerCallContext context)
    {
        _logger.LogTrace($"{nameof(WriteWChar)} invoked via GRPC");

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
        
        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            const string message = $"The tag name is invalid";

            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});

            context.Status = new Status(StatusCode.InvalidArgument, message);

            return new ();
        }

        try
        {
            var bytes = request.Value.ToByteArray();
            plcInstance.WriteWChar(request.TagName, Encoding.UTF8.GetString(bytes)[0]);

            return new()
            {
                Instance = new InstanceInfo(plcInstance),
                TagName = request.TagName,
            };
        }
        catch (SimulationRuntimeException e)
        {
            const string message = $"There was an error writing the value to the plc instance";

            _logger.LogError(e, message);
       
            await context.WriteResponseHeadersAsync(new() {new(MessageHeader, message)});
            context.Status = Status.DefaultCancelled;
        }

        return new ();   
    }

#endregion

}