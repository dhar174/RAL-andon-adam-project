using RAL.Manager.Configuration;
using RAL.Devices.Adam.Mocks;

//namespace CS_ScriptTest
//{
    public class UserConfigurationTest : UserConfigBase
    {
        public override void UserConfigLoad()
        {
            var test = new RAL.Devices.Adam.Mocks.Adam6051StatusConverterForPressMock(1);
        }
    }
//}
