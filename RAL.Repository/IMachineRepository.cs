namespace RAL.Repository
{
    public interface IMachineRepository
    {
        MachineIsConnectedRepo MachineIsConnectedRepo { get; }
        IMachineIsRunningRepository MachineIsRunningRepo { get; }
        IMachineStatusRepository MachineStatusRepo { get; }

        bool CanConnect();
        void CreateDB(string v);
        bool DoesDBExist(string databaseName);
        void DropDB(string v);
        void ThrowIfCantConnect();
    }
}