using Newtonsoft.Json;
using RAL.Collector;
using System;

namespace RAL.Devices.Adam.Mocks
{

    public class Adam6051StatusConverterForPressMock : IMachineStatusPayloadConverter
    {
        private int _inputNumber;
        private bool _isInAutomatic;
        private bool _isFaulted;

        public Adam6051StatusConverterForPressMock(int inputNumber, bool IsInAutomatic = true, bool IsFaulted = false)
        {
            _inputNumber = inputNumber;
            _isInAutomatic = IsInAutomatic;
            _isFaulted = IsFaulted;
    }

        public MachineStatusMessage Convert(string payload)
        {
            MachineStatusMessage newPressStatusData = new MachineStatusMessage();

            Adam6051DataPayloadRaw adamStatus = JsonConvert.DeserializeObject<Adam6051DataPayloadRaw>(payload);


            bool IsCycling = false;
            switch (_inputNumber)
            {
                case 1:
                    IsCycling = adamStatus.di1;
                    break;
                case 2:
                    IsCycling = adamStatus.di2;
                    break;
                case 3:
                    IsCycling = adamStatus.di3;
                    break;
                case 4:
                    IsCycling = adamStatus.di4;
                    break;
                case 5:
                    IsCycling = adamStatus.di5;
                    break;
                case 6:
                    IsCycling = adamStatus.di6;
                    break;
                case 7:
                    IsCycling = adamStatus.di7;
                    break;
                case 8:
                    IsCycling = adamStatus.di8;
                    break;
                case 9:
                    IsCycling = adamStatus.di9;
                    break;
                case 10:
                    IsCycling = adamStatus.di10;
                    break;
                case 11:
                    IsCycling = adamStatus.di11;
                    break;
                case 12:
                    IsCycling = adamStatus.di12;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Input must be between 1 and 12 (0 < input < 13)");
            }

            newPressStatusData.IsInAutomatic = _isInAutomatic;
            newPressStatusData.IsFaulted = _isFaulted;

            //** ATTENTION this is INVERTED
            newPressStatusData.IsCycling = !IsCycling;

            return newPressStatusData;
        }
    }


    public class PressAdam6051MessageConverterMock2 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock2() : base(2) { }
    }

    public class PressAdam6051MessageConverterMock3 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock3() : base(3) { }
    }

    public class PressAdam6051MessageConverterMock4 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock4() : base(4) { }
    }

    public class PressAdam6051MessageConverterMock5 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock5() : base(5) { }
    }

    public class PressAdam6051MessageConverterMock6 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock6() : base(6) { }
    }

    public class PressAdam6051MessageConverterMock7 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock7() : base(7) { }
    }

    public class PressAdam6051MessageConverterMock8 : Adam6051StatusConverterForPressMock
    {
        public PressAdam6051MessageConverterMock8() : base(8) { }
    }


}
