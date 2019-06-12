namespace RAL.Repository.Model
{ 
    public interface IMachineInfo
    {
        string IPAddress { get; set; }
        string MAC { get; set; }
        string Department { get; set; }
        string Line { get; set; }
        string Name { get; set; }

        //bool IsConnected { get; set; }
    }
}