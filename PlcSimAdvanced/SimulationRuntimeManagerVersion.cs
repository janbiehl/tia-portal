using System.Runtime.InteropServices;

namespace PlcSimAdvanced;

/// <summary>
/// Represents the Major and Minor version that are used by the PlcSimAdvanced Interface
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly record struct SimulationRuntimeManagerVersion
{
    // We are forcing the memory layout, so that Major and Minor are pointing to
    // the Memory where the high and low words are stored
    
    /// <summary>
    /// Used to construct a new version
    /// </summary>
    /// <param name="version">The version of the SimulationRuntimeManager</param>
    public SimulationRuntimeManagerVersion(uint version)
    {
        Major = 0;  // This line is used to prevent compiler errors
        Minor = 0;  // This line is used to prevent compiler errors
        Version = version;
    }

    /// <summary>
    /// This is the Version that will come from the Plc Sim Advanced interface
    /// </summary>
    [FieldOffset(0)]
    private readonly uint Version;
    
    /// <summary>
    /// The major version from the SimulationRuntimeManager
    /// </summary>
    [field: FieldOffset(0)]
    public ushort Major { get; }

    /// <summary>
    /// The minor version from the SimulationRuntimeManager
    /// </summary>
    [field: FieldOffset(2)]
    public ushort Minor { get; }
    
    /// <summary>
    /// Is the version valid
    /// </summary>
    public bool Valid => Major != 0;
    
    /// <summary>
    /// Get a string that represents the version
    /// </summary>
    public override string ToString()
    {
        return $"{Major}.{Minor}";
    }
}