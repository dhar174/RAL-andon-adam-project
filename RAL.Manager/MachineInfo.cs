using RAL.Repository.Model;

namespace RAL.Manager
{
    public class MachineInfo : IMachineInfo
    {
        public string IPAddress { get; set; }
        public string MAC { get; set; }
        public string Line { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public override string ToString()
        {
            return $"{Line}.{Name}";
        }
    }
}
