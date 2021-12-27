using Siemens.Simatic.Simulation.Runtime;

namespace PlcSimAdvanced;

public class RuntimeManagerGateway
{

#region Properties

    /// <summary>
    /// The version from ISimulationRuntimeManager
    /// </summary>
    public SimulationRuntimeManagerVersion Version { get; }

    /// <summary>
    /// Is the Plc Sim Advanced API initialized
    /// </summary>
    public bool IsInitialized => SimulationRuntimeManager.IsInitialized;

    /// <summary>
    /// Is there a Runtime Manager connected, this one should always be true.
    /// When it is false the Simulation was closed
    /// </summary>
    public bool IsRuntimeManagerAvailable => SimulationRuntimeManager.IsRuntimeManagerAvailable;
    
#endregion

#region Methods

    /// <summary>
    /// Close the connection with the Runtime Manager.
    /// Only call this when there is no more need to communicate with the Runtime Manager
    /// </summary>
    public void Shutdown()
    {
        if (IsRuntimeManagerAvailable)
            SimulationRuntimeManager.Shutdown();
    }

    /// <summary>
    /// Contains every already registered instances for the Runtime Manager. A new Instance can be created via <see cref="GetInstance(string)"/>
    /// </summary>
    /// <returns>The registered instances from the Runtime Manager</returns>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public SInstanceInfo[] GetRegisteredInstances() => SimulationRuntimeManager.RegisteredInstanceInfo;

    /// <summary>
    /// Register a new instance of a virtual controller in the Runtime Manager
    /// </summary>
    /// <returns>The interface for the new instance</returns>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public PlcInstance RegisterInstance()
    {
        var instance = SimulationRuntimeManager.RegisterInstance();

        return new PlcInstance(instance);
    }

    /// <summary>
    /// Register a new instance of a virtual controller in the Runtime Manager
    /// </summary>
    /// <param name="instanceName">The name for the new instance</param>
    /// <returns>The interface for the new instance</returns>
    /// <exception cref="ArgumentException">A argument is not valid</exception>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public PlcInstance RegisterInstance(string instanceName)
    {
        if (string.IsNullOrWhiteSpace(instanceName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(instanceName));
        
        var instance = SimulationRuntimeManager.RegisterInstance(instanceName);
        return new PlcInstance(instance);
    }

    /// <summary>
    /// Register a new instance of a virtual controller in the Runtime Manager
    /// </summary>
    /// <param name="cpuType">The cpu type that will be used for the virtual controller</param>
    /// <returns>The interface for the new instance</returns>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public PlcInstance RegisterInstance(ECPUType cpuType)
    {
        var instance = SimulationRuntimeManager.RegisterInstance(cpuType);

        return new PlcInstance(instance);
    }

    /// <summary>
    /// Register a new instance of a virtual controller in the Runtime Manager
    /// </summary>
    /// <param name="cpuType">The cpu type that will be used for the virtual controller</param>
    /// <param name="instanceName">The name for the new instance</param>
    /// <returns>The interface for the new instance</returns>
    /// <exception cref="ArgumentException">A argument is not valid</exception>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public PlcInstance RegisterInstance(ECPUType cpuType, string instanceName)
    {
        if (string.IsNullOrWhiteSpace(instanceName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(instanceName));
        
        var instance = SimulationRuntimeManager.RegisterInstance(cpuType, instanceName);
        return new PlcInstance(instance);
    }

    /// <summary>
    /// Find a instance of a virtual controller in the Runtime Manager via its name
    /// </summary>
    /// <param name="instanceName">The name for the instance</param>
    /// <returns>The interface for the found Instance, or null when it wasn't found</returns>
    /// <exception cref="ArgumentException">A argument is not valid</exception>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public PlcInstance? GetInstance(string instanceName)
    {
        if (string.IsNullOrWhiteSpace(instanceName))
            throw new ArgumentException(ExceptionUtils.InvalidStringMessage, nameof(instanceName));

        try
        {
            var instance = SimulationRuntimeManager.CreateInterface(instanceName);
            return new PlcInstance(instance);
        }
        catch (SimulationRuntimeException e) when (e.RuntimeErrorCode == ERuntimeErrorCode.DoesNotExist)
        { 
            return null;
        }
    }
    
    /// <summary>
    /// Find a instance of a virtual controller in the Runtime Manager via its ID
    /// </summary>
    /// <param name="instanceId">The id for the instance</param>
    /// <returns>The interface for the found Instance, or null when it wasn't found</returns>
    /// <exception cref="SimulationRuntimeException">Get detailed information from the error code</exception>
    public PlcInstance? GetInstance(int instanceId)
    {
        try
        {
            var instance =  SimulationRuntimeManager.CreateInterface(instanceId);

            return new PlcInstance(instance);
        }
        catch (SimulationRuntimeException e) when (e.RuntimeErrorCode == ERuntimeErrorCode.DoesNotExist)
        {
            return null;
        }
    }
    
    

#endregion
    
#region CTOR

    public RuntimeManagerGateway()
    {
        Version = new SimulationRuntimeManagerVersion(SimulationRuntimeManager.Version);
    }

#endregion
    
}