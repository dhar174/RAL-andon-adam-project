namespace RAL.Collector
{
    public interface IMachineStatusPayloadConverter
    {
        MachineStatusMessage Convert(string payload);
    }
}