// ReSharper disable once CheckNamespace
namespace PlcSimAdvanced.Protos
{
    public partial class InstanceInfo
    {
        public InstanceInfo(PlcInstance plcInstance)
        {
            Name = plcInstance.Name;
            Id = plcInstance.Id;
        }
        
    }
}
