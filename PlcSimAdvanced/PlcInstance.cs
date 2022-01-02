using Siemens.Simatic.Simulation.Runtime;

namespace PlcSimAdvanced;

public sealed class PlcInstance : IDisposable
{
    public const uint DefaultTimeout = 60000;
    private readonly IInstance _instance;

#region Common

    /// <summary>
    /// The instance ID that is used to identify the instance in the runtime manager
    /// </summary>
    public int Id
    {
#if DEBUG_MAC
        get { return 0; }
#else
        get { return _instance.ID; }
#endif
    }

    /// <summary>
    /// The name that identifies the instance in the runtime manager
    /// </summary>
    public string Name
    {
#if DEBUG_MAC
        get { return "Unspecific"; }
#else
        get { return _instance.Name; }
#endif
    }
    
    /// <summary>
    /// The type of cpu that is used by the instance
    /// </summary>
    public ECPUType CpuType
    {
#if DEBUG_MAC
        get => ECPUType.CPU1500_Unspecified;
        set { }
#else
        get => _instance.CPUType;
        set => _instance.CPUType = value;
#endif
    }

    /// <summary>
    /// The interface that is used for the communication
    /// </summary>
    public ECommunicationInterface CommunicationInterface
    {
#if DEBUG_MAC
        get { return ECommunicationInterface.None; }
#else
        get { return _instance.CommunicationInterface; }
#endif
    }

    /// <summary>
    /// Removes the instance from the runtime manager
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Unregister()
    {
        // TODO: Log the call

#if DEBUG_MAC
        return;
#endif
        
      if (_instance is null)
            throw new InvalidOperationException("The instance is not set up properly");
        
        _instance.UnregisterInstance();
    }

#endregion

#region Controller

    /// <summary>
    /// The name from controller
    /// </summary>
    public string ControllerName
    {
#if DEBUG_MAC
        get { return "Default Name"; }
#else
        get { return _instance.ControllerName; }
#endif
    }

    /// <summary>
    /// The short description from controller
    /// </summary>
    public string ControllerShortDesignation
    {
#if DEBUG_MAC
        get { return "Default designation"; }
#else
        get { return _instance.ControllerShortDesignation; }
#endif
    }

    /// <summary>
    /// The ip address from the controller, or null when there was an error
    /// </summary>
    public string[]? ControllerIp
    {
#if DEBUG_MAC
        get { return new[] {"0", "0", "0", "0"}; }
#else
        get { return _instance.ControllerIP.Length is 0 ? null : _instance.ControllerIP; }
#endif
    }

    /// <summary>
    /// The complete information for the ip addresses, or null when there was an errors
    /// </summary>
    public SIPSuite4[]? ControllerIpSuite
    {
#if DEBUG_MAC
        get { return null; }
#else
        get { return _instance.ControllerIPSuite4.Length is 0 ? null : _instance.ControllerIPSuite4; }
#endif
    }

    /// <summary>
    /// The path where the permanent data is saved on disk. Could be a network drive.
    /// </summary>
    /// <remarks>Applied after a controller restart</remarks>
    /// <default>DOCUMENTS\Siemens\Simatic\Simulation\Runtime\Persistence\INSTANCE_NAME </default>
    public string StoragePath
    {
#if DEBUG_MAC
        get => "Dummy-path";
#else
        get => _instance.StoragePath;
#endif
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(ExceptionUtils.InvalidStringMessage);
            
#if !DEBUG_MAC
            _instance.StoragePath = value;   
#endif
        }
    }

    /// <summary>
    /// Create a archive that contains the user defined software,
    /// the hardware configuration and the persistent data from virtual controller
    /// </summary>
    /// <param name="filePath">The complete path for the archive file</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    /// <see cref="PlcInstance.RetrieveStorage"/>
    public void ArchiveStorage(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(filePath));

        //if (!Path.HasExtension(filePath))
        //    throw new ArgumentException("There is no file extension", nameof(filePath));

        if (_instance.OperatingState != EOperatingState.Off)
            throw new InvalidOperationException(
                "The virtual controller must be turned OFF to be able to create the archive ");

#if DEBUG_MAC
        return;
#endif
        
        _instance.ArchiveStorage(filePath);
    }

    /// <summary>
    /// Uses the archive to restore the state for the virtual controller
    /// </summary>
    /// <remarks>The virtual controller must be turned OFF</remarks>
    /// <param name="filePath">The complete path for the archive file</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <see cref="ArchiveStorage"/>
    public void RetrieveStorage(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("The file could not be found", filePath);

        if (_instance.OperatingState != EOperatingState.Off)
            throw new InvalidOperationException(
                "The virtual controller must be turned OFF to be able to retrieve the state from the archive");
        
#if DEBUG_MAC
        return;
#endif
        
        _instance.RetrieveStorage(filePath);
    }
    
    public void CleanupStoragePath()
    {
        if (_instance.OperatingState != EOperatingState.Off)
            throw new InvalidOperationException(
                "The virtual controller must be turned OFF to be able to remove the virtual memory card");

#if DEBUG_MAC
        return;
#endif
        
        _instance.CleanupStoragePath();
    }

#endregion

#region Operation state

    /// <summary>
    /// The current operating state the virtual controller is in
    /// </summary>
    public EOperatingState OperatingState
    {
#if DEBUG_MAC
        get { return EOperatingState.Off; }
#else
        get { return _instance.OperatingState; }
#endif
    }


    /// <summary>
    /// Start the process for the virtual controller
    /// </summary>
    /// <param name="timeout">The timeout in milliseconds</param>
    /// <returns>The status code</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <see cref="PowerOff"/>
    public ERuntimeErrorCode PowerOn(uint timeout = DefaultTimeout)
    {
        if (timeout <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeout),
                "The timeout must greater than 0, 60000 is recommended");
        
#if DEBUG_MAC
        return ERuntimeErrorCode.OK;
#endif
        
        var errorCode = _instance.PowerOn(timeout);

        return errorCode;
    }

    /// <summary>
    /// Stop the process for the virtual controller
    /// </summary>
    /// <param name="timeout">The timeout in milliseconds</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void PowerOff(uint timeout = DefaultTimeout)
    {
        if (timeout <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeout),
                "The timeout must greater than 0, 60000 is recommended");

#if DEBUG_MAC
        return;
#endif
        
        _instance.PowerOff(timeout);
    }
    
    /// <summary>
    /// Set the operation state to Run
    /// </summary>
    /// <param name="timeout">The timeout in milliseconds</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Run(uint timeout = DefaultTimeout)
    {        
        if (timeout <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeout),
                "The timeout must greater than 0, 60000 is recommended");

        if (OperatingState is EOperatingState.Run or EOperatingState.Startup)
            return;
        
#if DEBUG_MAC
        return;
#endif
        
        _instance.Run(timeout);
    }

    /// <summary>
    /// Set the operation state to Stop
    /// </summary>
    /// <param name="timeout">The timeout in milliseconds</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Stop(uint timeout = DefaultTimeout)
    {
        if (timeout <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeout),
                "The timeout must greater than 0, 60000 is recommended");
        
        if (OperatingState is EOperatingState.Stop)
            return;
        
#if DEBUG_MAC
        return;
#endif
        
        _instance.Stop(timeout);
    }

    /// <summary>
    /// Reset the virtual controller to it's default state
    /// </summary>
    /// <param name="timeout">The timeout in milliseconds</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void MemoryRest(uint timeout = DefaultTimeout)
    {
        if (timeout <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeout),
                "The timeout must greater than 0, 60000 is recommended");
        
#if DEBUG_MAC
        return;
#endif
        
        _instance.MemoryReset();
    }

#endregion

#region Tagtable

    /// <summary>
    /// The filtered list with tags from the virtual controller
    /// </summary>
    /// <exception cref="SimulationRuntimeException"></exception>
    public STagInfo[] TagInfos
    {
#if DEBUG_MAC
        get => Array.Empty<STagInfo>();
#else
        get => _instance.TagInfos;
#endif
    }

    /// <summary>
    /// Read the tag tables from the virtual controller 
    /// </summary>
    /// <param name="filter">The areas which should be included from tag lists</param>
    /// <param name="hmiVisibleOnly">When set to true, only tags that are marked as hmi visible will be read</param>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void UpdateTagList(ETagListDetails filter = ETagListDetails.IOM, bool hmiVisibleOnly = false)
    {
#if DEBUG_MAC
        return;
#endif
        
        _instance.UpdateTagList(filter, hmiVisibleOnly);
    }
    
    /// <summary>
    /// Read the tag tables from the virtual controller
    /// </summary>
    /// <param name="dataBlockFilter">Data block names that should be included in the tag table</param>
    /// <param name="filter">The areas which should be included from tag lists</param>
    /// <param name="hmiVisibleOnly">When set to true, only tags that are marked as hmi visible will be read</param>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void UpdateTagList(string[] dataBlockFilter, ETagListDetails filter = ETagListDetails.IOMDB, bool hmiVisibleOnly = false)
    {
        // TODO: Validate the utility function 
        var dataBlockFilterString = Utils.GetDataBlockFilter(dataBlockFilter);
        
#if DEBUG_MAC
        return;
#endif
        
        _instance.UpdateTagList(filter, hmiVisibleOnly, dataBlockFilterString);
    }
    
    /// <summary>
    /// Get the current status for the tag list
    /// </summary>
    /// <param name="details">The resulting details, when None a <see cref="UpdateTagList"/> is required</param>
    /// <param name="hmiVisibleOnly">When set to true, only tags that are marked as hmi visible are visible</param>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void GetTagListStatus(out ETagListDetails details, out bool hmiVisibleOnly)
    {
#if DEBUG_MAC
        details = ETagListDetails.IOMCTDB;
        hmiVisibleOnly = false;
        
        return;
#endif
        
        _instance.GetTagListStatus(out details, out hmiVisibleOnly);
    }

    /// <summary>
    /// Writes all entries from the tag list into a XML file
    /// </summary>
    /// <param name="filePath">The full path to the xml file</param>
    /// <param name="overwriteFile">When true an existing file will be overwritten</param>
    /// <exception cref="SimulationRuntimeException"></exception>
    public bool CreateConfigurationFile(string filePath, bool overwriteFile = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(filePath));

        if (!Path.HasExtension(filePath))
            throw new ArgumentException("The filePath does not contain a filePath", nameof(filePath));

        if (File.Exists(filePath))
        {
            if (!overwriteFile)
                return false;

            File.Delete(filePath);
        }

#if DEBUG_MAC
        return true;
#endif

        _instance.CreateConfigurationFile(filePath);
        return true;
    }

#endregion
    
#region I/O

    /// <summary>
    /// The input area of the virtual controller
    /// </summary>
    public IIOArea Inputs
    {
#if DEBUG_MAC
        get => null;
#else
        get => _instance.InputArea;
#endif
    }

    /// <summary>
    /// The output area of the virtual controller
    /// </summary>
    public IIOArea Outputs
    {
#if DEBUG_MAC
        get => null;
#else
        get => _instance.OutputArea;
#endif
    }

    /// <summary>
    /// The marker area of the virtual controller
    /// </summary>
    public IIOArea Markers
    {
#if DEBUG_MAC
        get => null;
#else
        get => _instance.MarkerArea;
#endif
    }

#region ByAddress
    
    /// <summary>
    /// Read a single bit from a memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="area">The memory area to use for the read</param>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bit">The bit offset in memory, 0 to 7</param>
    /// <returns>The value for the requested bit</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public bool ReadBit(IIOArea area, uint offset, byte bit)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (offset > area.AreaSize)
            throw new ArgumentOutOfRangeException(nameof(offset), 
                "The offset must be between 0 and the areas size");

        if (bit is < 0 or > 7)
            throw new ArgumentOutOfRangeException(nameof(bit),
                "Values smaller than 0 or greater than 7 are not allowed");
        
        return area.ReadBit(offset, bit);
    }

    /// <summary>
    /// Read a single bit from the Input memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bit">The bit offset in memory, 0 to 7</param>
    /// <returns>The value for the requested bit</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public bool ReadInputBit(uint offset, byte bit) => ReadBit(Inputs, offset, bit);
    
    /// <summary>
    /// Read a single bit from the marker memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bit">The bit offset in memory, 0 to 7</param>
    /// <returns>The value for the requested bit</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public bool ReadMarkerBit(uint offset, byte bit) => ReadBit(Markers, offset, bit);
    
    /// <summary>
    /// Read a single bit from the output memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bit">The bit offset in memory, 0 to 7</param>
    /// <returns>The value for the requested bit</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public bool ReadOutputBit(uint offset, byte bit) => ReadBit(Outputs, offset, bit);

    /// <summary>
    /// Read a byte value from a memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="area">The memory area to use for the read</param>
    /// <param name="offset">The byte offset in memory</param>
    /// <returns>The value for the requested byte</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte ReadByte(IIOArea area, uint offset)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (offset > area.AreaSize)
            throw new ArgumentOutOfRangeException(nameof(offset), 
                "The offset must be between 0 and the areas size");

        return area.ReadByte(offset);
    }

    /// <summary>
    /// Read a byte value from the input memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <returns>The value for the requested byte</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte ReadInputByte(uint offset) => ReadByte(Inputs, offset);
    
    /// <summary>
    /// Read a byte value from the marker memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <returns>The value for the requested byte</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte ReadMarkerByte(uint offset) => ReadByte(Markers, offset);
    
    /// <summary>
    /// Read a byte value from the output memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <returns>The value for the requested byte</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte ReadOutputByte(uint offset) => ReadByte(Outputs, offset);
    
    /// <summary>
    /// Read multiple byte values from a memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="area">The memory area to use for the read</param>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bytesToRead">The amount of bytes to read</param>
    /// <returns>The bytes read from the virtual controller</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte[] ReadBytes(IIOArea area, uint offset, uint bytesToRead)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (offset > area.AreaSize)
            throw new ArgumentOutOfRangeException(nameof(offset), 
                "The offset must be between 0 and the areas size");

        if ((offset + bytesToRead) > area.AreaSize)
            throw new ArgumentOutOfRangeException(
                $"The sum of '{nameof(offset)}' and '{nameof(bytesToRead)}' is to large for the memory area " +
                "of the virtual controller. Lower the offset or reduce the bytes to read");
        
        return area.ReadBytes(offset, bytesToRead);
    }

    /// <summary>
    /// Read multiple byte values from input memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bytesToRead">The amount of bytes to read</param>
    /// <returns>The bytes read from the virtual controller</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte[] ReadInputBytes(uint offset, uint bytesToRead) => ReadBytes(Inputs, offset, bytesToRead);
    
    /// <summary>
    /// Read multiple byte values from marker memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bytesToRead">The amount of bytes to read</param>
    /// <returns>The bytes read from the virtual controller</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte[] ReadMarkerBytes(uint offset, uint bytesToRead) => ReadBytes(Markers, offset, bytesToRead);
    
    /// <summary>
    /// Read multiple byte values from output memory area
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="offset">The byte offset in memory</param>
    /// <param name="bytesToRead">The amount of bytes to read</param>
    /// <returns>The bytes read from the virtual controller</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public byte[] ReadOutputBytes(uint offset, uint bytesToRead) => ReadBytes(Outputs, offset, bytesToRead);
    
    /// <summary>
    /// Read a structured list from virtual controller.
    /// Structs and fields can be grouped into a structured list.
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="area">The memory area to use for the read</param>
    /// <param name="signals">The signal list</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadSignals(IIOArea area, ref SDataValueByAddress[] signals)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));

        if (signals == null) 
            throw new ArgumentNullException(nameof(signals));
        
        if (signals.Length == 0) 
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(signals));
        
        area.ReadSignals(ref signals);
    }

    /// <summary>
    /// Read a structured list from virtual controller.
    /// Structs and fields can be grouped into a structured list.
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="signals">The signal list</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadInputSignals(ref SDataValueByAddress[] signals) => ReadSignals(Inputs, ref signals);
    
    /// <summary>
    /// Read a structured list from virtual controller.
    /// Structs and fields can be grouped into a structured list.
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="signals">The signal list</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadMarkerSignals(ref SDataValueByAddress[] signals) => ReadSignals(Markers, ref signals);
    
    /// <summary>
    /// Read a structured list from virtual controller.
    /// Structs and fields can be grouped into a structured list.
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="signals">The signal list</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadOutputSignals(ref SDataValueByAddress[] signals) => ReadSignals(Outputs, ref signals);
    
    /// <summary>
    /// Read a list of multiple values from the virtual controller.
    /// Structs and fields can be grouped in this list. At the same time a check will be performed,
    /// weather or not a value has changed
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="area">The memory area to use for the read</param>
    /// <param name="signals">The signals that are contained in the list</param>
    /// <param name="valueHasChanged">True when at least one value has changed</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadSignals(IIOArea area, ref SDataValueByAddressWithCheck[] signals, out bool valueHasChanged)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));

        if (signals == null) 
            throw new ArgumentNullException(nameof(signals));
        
        if (signals.Length == 0) 
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(signals));
        
        area.ReadSignals(ref signals, out valueHasChanged);
    }

    /// <summary>
    /// Read a list of multiple values from the virtual controller.
    /// Structs and fields can be grouped in this list. At the same time a check will be performed,
    /// weather or not a value has changed
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="signals">The signals that are contained in the list</param>
    /// <param name="valueHasChanged">True when at least one value has changed</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadInputSignals(ref SDataValueByAddressWithCheck[] signals, out bool valueHasChanged) =>
        ReadSignals(Inputs, ref signals, out valueHasChanged);
    
    /// <summary>
    /// Read a list of multiple values from the virtual controller.
    /// Structs and fields can be grouped in this list. At the same time a check will be performed,
    /// weather or not a value has changed
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="signals">The signals that are contained in the list</param>
    /// <param name="valueHasChanged">True when at least one value has changed</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadMarkerSignals(ref SDataValueByAddressWithCheck[] signals, out bool valueHasChanged) =>
        ReadSignals(Markers, ref signals, out valueHasChanged);
    
    /// <summary>
    /// Read a list of multiple values from the virtual controller.
    /// Structs and fields can be grouped in this list. At the same time a check will be performed,
    /// weather or not a value has changed
    /// </summary>
    /// <remarks>
    /// This function allows access to the whole memory area of the virtual controller.
    /// You should avoid using this method and use the one with variable name instead.
    /// </remarks>
    /// <param name="signals">The signals that are contained in the list</param>
    /// <param name="valueHasChanged">True when at least one value has changed</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SimulationRuntimeException"></exception>
    public void ReadOutputSignals(ref SDataValueByAddressWithCheck[] signals, out bool valueHasChanged) =>
        ReadSignals(Outputs, ref signals, out valueHasChanged);
    
    public void WriteBit(IIOArea area, uint offset, byte bit, bool value)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (offset > area.AreaSize)
            throw new ArgumentOutOfRangeException(nameof(offset), 
                "The offset must be between 0 and the areas size");

        if (bit is < 0 or > 7)
            throw new ArgumentOutOfRangeException(nameof(bit),
                "Values smaller than 0 or greater than 7 are not allowed");

        area.WriteBit(offset, bit, value);
    }

    public void WriteInputBit(uint offset, byte bit, bool value) => WriteBit(Inputs, offset, bit, value);
    public void WriteMarkerBit(uint offset, byte bit, bool value) => WriteBit(Markers, offset, bit, value);
    public void WriteOutputBit(uint offset, byte bit, bool value) => WriteBit(Outputs, offset, bit, value);

    public void WriteByte(IIOArea area, uint offset, byte value)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (offset > area.AreaSize)
            throw new ArgumentOutOfRangeException(nameof(offset), 
                "The offset must be between 0 and the areas size");

        area.WriteByte(offset, value);   
    }
    
    public void WriteInputByte(uint offset, byte value) => WriteByte(Inputs, offset, value);
    public void WriteMarkerByte(uint offset, byte value) => WriteByte(Markers, offset, value);
    public void WriteOutputByte(uint offset, byte value) => WriteByte(Outputs, offset, value);
    
    public void WriteBytes(IIOArea area, uint offset, byte[] values)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (offset > area.AreaSize)
            throw new ArgumentOutOfRangeException(nameof(offset), 
                "The offset must be between 0 and the areas size");

        if ((offset + values.Length) > area.AreaSize)
            throw new ArgumentOutOfRangeException(
                $"The sum of '{nameof(offset)}' and '{nameof(values).Length}' is to large for the memory area " +
                "of the virtual controller. Lower the offset or reduce the bytes to write");

        area.WriteBytes(offset, values);
    }

    public void WriteInputBytes(uint offset, byte[] values) => WriteBytes(Inputs, offset, values);
    public void WriteMarkerBytes(uint offset, byte[] values) => WriteBytes(Markers, offset, values);
    public void WriteOutputBytes(uint offset, byte[] values) => WriteBytes(Outputs, offset, values);

    public void WriteSignals(IIOArea area, ref SDataValueByAddress[] signals)
    {
        if (area is null)
            throw new ArgumentNullException(nameof(area));
        
        if (signals == null) 
            throw new ArgumentNullException(nameof(signals));
        
        if (signals.Length == 0) 
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(signals));

        area.WriteSignals(ref signals);
    }

    public void WriteInputSignals(ref SDataValueByAddress[] signals) => WriteSignals(Inputs, ref signals);
    public void WriteMarkerSignals(ref SDataValueByAddress[] signals) => WriteSignals(Markers, ref signals);
    public void WriteOutputSignals(ref SDataValueByAddress[] signals) => WriteSignals(Outputs, ref signals);

#endregion

#region ByName
    
    public SDataValue Read(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return new SDataValue();
#endif
        
        return _instance.Read(tagName);
    }
    
    public bool ReadBool(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return false;
#endif
        
        return _instance.ReadBool(tagName);
    }
    
    public sbyte ReadChar(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return (sbyte) Char.MinValue;
#endif
        
        return _instance.ReadChar(tagName);
    } 
    
    public char ReadWChar(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return Char.MinValue;
#endif  
        
        return _instance.ReadWChar(tagName);
    }
    
    public double ReadDouble(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return 0.0;
#endif
        
        return _instance.ReadDouble(tagName);
    }
    
    public float ReadFloat(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0f;
#endif
        
        return _instance.ReadFloat(tagName);
    }

    public sbyte ReadInt8(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadInt8(tagName);
    } 
    
    public short ReadInt16(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadInt16(tagName);
    } 
    
    public int ReadInt32(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadInt32(tagName);
    } 
    
    public long ReadInt64(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadInt64(tagName);
    }
    
    public byte ReadUInt8(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadUInt8(tagName);
    } 
    
    public ushort ReadUInt16(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadUInt16(tagName);
    } 
    
    public uint ReadUInt32(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadUInt32(tagName);
    } 
    
    public ulong ReadUInt64(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return 0;
#endif
        
        return _instance.ReadUInt64(tagName);
    }

    public void ReadSignals(ref SDataValueByName[] signals)
    {
        if (signals == null) 
            throw new ArgumentNullException(nameof(signals));
        
        if (signals.Length == 0) 
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(signals));

#if DEBUG_MAC
        return;
#endif
        
        _instance.ReadSignals(ref signals);
    }
    
    public void ReadSignals(ref SDataValueByNameWithCheck[] signals, out bool signalsHaveChanged)
    {
        if (signals == null) 
            throw new ArgumentNullException(nameof(signals));
        
        if (signals.Length == 0) 
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(signals));

#if DEBUG_MAC
        signalsHaveChanged = false;
        return;
#endif
        
        _instance.ReadSignals(ref signals, out signalsHaveChanged);
    }

    public void Write(string tagName, SDataValue dataValue)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.Write(tagName, dataValue);
    }

    public void WriteBool(string tagName, bool value)
    {  
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteBool(tagName, value);
    }
    
    public void WriteChar(string tagName, sbyte value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
    
#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteChar(tagName, value);
    }
    
    public void WriteWChar(string tagName, char value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteWChar(tagName, value);
    }
    
    public void WriteDouble(string tagName, double value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteDouble(tagName, value);
    }
    
    public void WriteFloat(string tagName, float value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteFloat(tagName, value);
    }
    
    public void WriteInt8(string tagName, sbyte value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteInt8(tagName, value);
    } 
    
    public void WriteInt16(string tagName, short value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
   
#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteInt16(tagName, value);
    }
    
    public void WriteInt32(string tagName, int value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteInt32(tagName, value);
    }
    
    public void WriteInt64(string tagName, long value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteInt64(tagName, value);
    }
    
    public void WriteUInt8(string tagName, byte value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif
        
        _instance.WriteUInt8(tagName, value);
    }
    
    public void WriteUInt16(string tagName, ushort value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif

        _instance.WriteUInt16(tagName, value);
    }
    
    public void WriteUInt32(string tagName, uint value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));
        
#if DEBUG_MAC
        return;
#endif

        _instance.WriteUInt32(tagName, value);
    }
    
    public void WriteUInt64(string tagName, ulong value)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(tagName));

#if DEBUG_MAC
        return;
#endif

        _instance.WriteUInt64(tagName, value);
    }

    public void WriteSignals(ref SDataValueByName[] signals)
    {
        if (signals == null) 
            throw new ArgumentNullException(nameof(signals));
        
        if (signals.Length == 0) 
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(signals));

#if DEBUG_MAC
        return;
#endif

        _instance.WriteSignals(ref signals);
    }
    
#endregion
    
#endregion

#region Virtual time

    public DateTime Time
    {
#if DEBUG_MAC
        get => DateTime.Now;
        set { }
#else
        get => _instance.SystemTime;
        set => _instance.SystemTime = value;
#endif
    }

    public double TimeScale
    {
#if DEBUG_MAC
        get => 1.0;
        set { }
#else
        get => _instance.ScaleFactor;
        set => _instance.ScaleFactor = value;
#endif
    }
    
#endregion

#region Cycle

    public EOperatingMode OperatingMode
    {
#if DEBUG_MAC
        get => EOperatingMode.Default;
        set { }
#else
        get => _instance.OperatingMode;
        set => _instance.OperatingMode = value;
#endif
    }

    public bool IsSendSyncEventInDefaultModeEnabled
    {
#if DEBUG_MAC
        get => true;
        set { }
#else
        get => _instance.IsSendSyncEventInDefaultModeEnabled;
        set => _instance.IsSendSyncEventInDefaultModeEnabled = value;
#endif
    }

    public long OverwrittenMinimalCycleTime
    {
#if DEBUG_MAC
        get => 0;
        set { }
#else
        get => _instance.OverwrittenMinimalCycleTime_ns; 
        set => _instance.OverwrittenMinimalCycleTime_ns = value;
#endif
    }

    public void RunToNextSyncPoint()
    {
#if DEBUG_MAC
        return;
#endif

        _instance.RunToNextSyncPoint();
    }

    public void StartProcessing(long runTime)
    {
#if DEBUG_MAC
        return;
#endif

        _instance.StartProcessing(runTime);
    }

    public void SetCycleTimeMonitoringMode(ECycleTimeMonitoringMode mode)
    {
        if (mode is ECycleTimeMonitoringMode.Specified)
            throw new InvalidOperationException(
                "To use the specific mode you have to use the overload with the cycle time");
        
#if DEBUG_MAC
        return;
#endif

        _instance.SetCycleTimeMonitoringMode(mode);
    }
    
    public void SetCycleTimeMonitoringMode(long maxCycleTime)
    {
#if DEBUG_MAC
        return;
#endif

        _instance.SetCycleTimeMonitoringMode(ECycleTimeMonitoringMode.Specified, maxCycleTime);
    }

    public void GetCycleTimeMonitoringMode(out long maxCycleTime, out ECycleTimeMonitoringMode monitoringMode)
    {
#if DEBUG_MAC
        maxCycleTime = 150000;
        monitoringMode = ECycleTimeMonitoringMode.Ignored;
        return;
#endif

        _instance.GetCycleTimeMonitoringMode(out monitoringMode, out maxCycleTime);
    }

#endregion
    
    public PlcInstance(IInstance instance)
    {
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public void Dispose()
    {
#if DEBUG_MAC
        return;
#endif

        _instance.Dispose();
    }
}