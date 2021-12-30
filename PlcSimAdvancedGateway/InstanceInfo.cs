namespace PlcSimAdvancedGateway;

public readonly record struct InstanceInfo
{
    public string Name { get; init; }
    public int Id { get; init; }
}