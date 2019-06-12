namespace RAL.Devices.StackLights
{
    public interface IStackLightRYGBW : IStackLight5Light
    {
        bool IsBlueLightOff { get; set; }
        bool IsBlueLightOn { get; set; }
        bool IsGreenLightOff { get; set; }
        bool IsGreenLightOn { get; set; }
        bool IsRedLightOff { get; set; }
        bool IsRedLightOn { get; set; }
        bool IsWhiteLightOff { get; set; }
        bool IsWhiteLightOn { get; set; }
        bool IsYellowLightOff { get; set; }
        bool IsYellowLightOn { get; set; }
    }
}