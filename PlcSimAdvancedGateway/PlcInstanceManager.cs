using PlcSimAdvanced;
using PlcSimAdvanced.Protos;

namespace PlcSimAdvancedGateway;

public class PlcInstanceManager
{
    private readonly Dictionary<InstanceInfo, PlcInstance> _plcInstances;
    private readonly SemaphoreSlim _plcInstancesSemaphore;
    
    public PlcInstanceManager()
    {
        _plcInstances = new();
        _plcInstancesSemaphore = new(0, 1);
    }

    public async Task<bool> AddPlcInstanceAsync(PlcInstance plcInstance, CancellationToken cancellationToken = default)
    {
        InstanceInfo instanceInfo = new(plcInstance);
        
        await _plcInstancesSemaphore.WaitAsync(cancellationToken);

        if (_plcInstances.ContainsKey(instanceInfo))
            return false;

        var result = _plcInstances.TryAdd(instanceInfo, plcInstance);

        _plcInstancesSemaphore.Release();

        return result;
    }

    public async Task<PlcInstance?> GetPlcInstanceAsync(
        InstanceInfoRequest instanceInfo, 
        CancellationToken cancellationToken = default)
    {
        switch (instanceInfo.DataOneofCase)
        {
            case InstanceInfoRequest.DataOneofOneofCase.Id:
            {
                if (instanceInfo.Id is 0)
                {
                    return null;
                }

                await _plcInstancesSemaphore.WaitAsync(cancellationToken);
                
                foreach (var (key, value) in _plcInstances)
                {
                    if (key.Id == instanceInfo.Id)
                        return value;
                }

                _plcInstancesSemaphore.Release();

                break;
            }
            case InstanceInfoRequest.DataOneofOneofCase.Name:
            {
                if (string.IsNullOrWhiteSpace(instanceInfo.Name))
                {
                    return null;
                }

                await _plcInstancesSemaphore.WaitAsync(cancellationToken);

                foreach (var (key, value) in _plcInstances)
                {
                    if (key.Name == instanceInfo.Name)
                        return value;
                }

                _plcInstancesSemaphore.Release();

                break;
            }
        }

        return null;
    }
}