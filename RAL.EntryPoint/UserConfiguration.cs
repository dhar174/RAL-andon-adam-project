using RAL.Devices.StackLights;
using RAL.Manager.Configuration;

//namespace RAL.EntryPoint
//{
    public class UserConfiguration : UserConfigBase
    {
        public override void UserConfigLoad()
        {
            //** Don't  mess with this
            _defaultDepartment = "Parts Productions";

            EmailServerConfiguration = new EmailServerConfiguration()
            {
                EmailServer = "172.16.20.31",
                EmailServerPort = 8026,
            };
            //** End Don't mess with this section

            MachineIsRunningGracePeriodInSeconds = 180;


            //** Define Machines/Presses and the MAC address for the ADAM Module

            //** Example
            //** var Z11 = AddMachine("Z-1-1","Press","FF-FF-FF-FF-FF")

            //      var F52 = AddMachine("F-5-2", "Press", "00-D0-C9-FC-A9-AE");
            //var G21 = AddMachine("G-1-2", "Press", "00-D0-C9-FC-A9-B5");
            //		var G61 = AddMachine("G-6-1", "Press", "00-D0-C9-FC-A9-B8"); //** Temp disabled because the module is not connected
            //var G22 = AddMachine("G-1-0", "Press", "00-D0-C9-FC-A9-B9");
            //		var G62 = AddMachine("G-6-2", "Press", "00-D0-C9-FC-A9-BB"); //** Temp disabled because the module is not connected
            var F73 = AddMachine("F-7-3", "Press", "00-D0-C9-FC-A9-BC");
            var G42 = AddMachine("G-4-2", "Press", "00-D0-C9-FC-A9-BD");
            var F46 = AddMachine("F-4-6", "Press", "00-D0-C9-FC-A9-BF");
            var F81 = AddMachine("F-8-1", "Press", "00-D0-C9-FC-A9-C0");
            var F32 = AddMachine("F-3-2", "Press", "00-D0-C9-FC-A9-C1");
            var F72 = AddMachine("F-7-2", "Press", "00-D0-C9-FC-A9-C3");
            var F82 = AddMachine("F-8-2", "Press", "00-D0-C9-FC-A9-C7");
            var G41 = AddMachine("G-4-1", "Press", "00-D0-C9-FC-AA-3F");
            var F21 = AddMachine("F-2-1", "Press", "00-D0-C9-FF-16-81");
            var F47 = AddMachine("F-4-7", "Press", "00-D0-C9-FF-16-93");
            //		var F63 = AddMachine("F-6-3", "Press", "00-D0-C9-FF-16-94"); //** Temp disabled because the module is not connected
            var F71 = AddMachine("F-7-1", "Press", "00-D0-C9-FF-16-95");
            var E73 = AddMachine("E-7-3", "Press", "00-D0-C9-FF-16-97");
            var F64 = AddMachine("F-6-4", "Press", "00-D0-C9-FF-16-99");
            var E66 = AddMachine("E-6-6", "Press", "00-D0-C9-FF-16-9C");
            //		var F22 = AddMachine("F-2-2", "Press", "00-D0-C9-FF-16-A3"); //** Temp disabled because the module is not connected
            var F62 = AddMachine("F-6-2", "Press", "00-D0-C9-FF-16-A4");
            //		var F31 = AddMachine("F-3-1", "Press", "00-D0-C9-FF-16-A5"); //** Temp disabled because the module is not connected
            var E65 = AddMachine("E-6-5", "Press", "00-D0-C9-FF-16-CC");
            var F45 = AddMachine("F-4-5", "Press", "00-D0-C9-FF-16-D0");
            var F33 = AddMachine("F-3-3", "Press", "00-D0-C9-FF-16-DE");

            //var E11 = AddMachine("E-1-1", "Press", "");
            //var E13 = AddMachine("E-1-3", "Press", "");
            //var E45 = AddMachine("E-4-5", "Press", "");
            //var E84 = AddMachine("E-8-4", "Press", "");
            //var F11 = AddMachine("F-1-1", "Press", "");
            //var F23 = AddMachine("F-2-3", "Press", "");
            //var F34 = AddMachine("F-3-4", "Press", "");
            //var F36 = AddMachine("F-3-6", "Press", "");
            //var F83 = AddMachine("F-8-3", "Press", "");
            //var F84 = AddMachine("F-8-4", "Press", "");
            //var F92 = AddMachine("F-9-2", "Press", "");
            //var G31 = AddMachine("G-3-1", "Press", "");
            //var G34 = AddMachine("G-3-4", "Press", "");
            //var G63 = AddMachine("G-6-3", "Press", "");
            //var G64 = AddMachine("G-6-4", "Press", "");
            //var G71 = AddMachine("G-7-1", "Press", "");
            //var G72 = AddMachine("G-7-2", "Press", "");
            //var G81 = AddMachine("G-8-1", "Press", "");
            //var G82 = AddMachine("G-8-2", "Press", "");
            //var G91 = AddMachine("G-9-1", "Press", "");
            //var H32 = AddMachine("H-3-2", "Press", "");



            var Row1StackLight = AddStackLight("172.16.28.159", "Row 1 Stack Light");
            var Row2StackLight = AddStackLight("172.16.28.160", "Row 2 Stack Light");
            var Row3StackLight = AddStackLight("172.16.28.161", "Row 3 Stack Light");
            var Row4StackLight = AddStackLight("172.16.28.162", "Row 4 Stack Light");
            var Row5StackLight = AddStackLight("172.16.28.154", "Row 5 Stack Light");
            var Row6StackLight = AddStackLight("172.16.28.153", "Row 6 Stack Light");
            var Row7StackLight = AddStackLight("172.16.28.152", "Row 7 Stack Light");
            var Row8StackLight = AddStackLight("172.16.28.151", "Row 8 Stack Light");
            var Row9StackLight = AddStackLight("172.16.28.158", "Row 9 Stack Light");
            var Row10StackLight = AddStackLight("172.16.28.157", "Row 10 Stack Light");
            var Row11StackLight = AddStackLight("172.16.28.156", "Row 11 Stack Light");
            var Row12StackLight = AddStackLight("172.16.28.155", "Row 12 Stack Light");

            //** Row 1
            //AddLightToMachineMap(G22, StackLight5Lights.LightNumber.Light0, Row1StackLight);
            //AddLightToMachineMap(G21, StackLight5Lights.LightNumber.Light1, Row1StackLight);
            //AddLightToMachineMap(F24, StackLight5Lights.LightNumber.Light2, Row1StackLight);
            //AddLightToMachineMap(F23, "Press", StackLight5Lights.LightNumber.Light3, Row1StackLight);

            //** Row 2
            //AddLightToMachineMap(G31, StackLight5Lights.LightNumber.Light0, Row2StackLight);
            //		AddLightToMachineMap(F22, StackLight5Lights.LightNumber.Light1, Row2StackLight);
            AddLightToMachineMap(F21, StackLight5Lights.LightNumber.Light2, Row2StackLight);

            //** Row 3
            //AddLightToMachineMap(G34, StackLight5Lights.LightNumber.Light0, Row3StackLight);
            //AddLightToMachineMap(F36, StackLight5Lights.LightNumber.Light1, Row3StackLight);
            AddLightToMachineMap(F33, StackLight5Lights.LightNumber.Light2, Row3StackLight);
            //		AddLightToMachineMap(F31, StackLight5Lights.LightNumber.Light3, Row3StackLight);

            //** Row 4
            AddLightToMachineMap(G42, StackLight5Lights.LightNumber.Light0, Row4StackLight);
            AddLightToMachineMap(F45, StackLight5Lights.LightNumber.Light1, Row4StackLight);
            AddLightToMachineMap(F32, StackLight5Lights.LightNumber.Light2, Row4StackLight);
            //AddLightToMachineMap(F34, StackLight5Lights.LightNumber.Light3, Row4StackLight);

            //** Row 5
            AddLightToMachineMap(G41, StackLight5Lights.LightNumber.Light0, Row5StackLight);
            AddLightToMachineMap(F47, StackLight5Lights.LightNumber.Light1, Row5StackLight);
            AddLightToMachineMap(F46, StackLight5Lights.LightNumber.Light2, Row4StackLight);
            //AddLightToMachineMap(E45, StackLight5Lights.LightNumber.Light3, Row4StackLight);

            //** Row 6
            AddLightToMachineMap(F72, StackLight5Lights.LightNumber.Light0, Row6StackLight);
            AddLightToMachineMap(E73, StackLight5Lights.LightNumber.Light1, Row6StackLight);
            AddLightToMachineMap(F62, StackLight5Lights.LightNumber.Light2, Row6StackLight);
            //  AddLightToMachineMap(F52, StackLight5Lights.LightNumber.Light3, Row6StackLight);

            //** Row 7
            //		AddLightToMachineMap(G61, StackLight5Lights.LightNumber.Light0, Row7StackLight);
            //		AddLightToMachineMap(G62, StackLight5Lights.LightNumber.Light1, Row7StackLight);
            AddLightToMachineMap(F64, StackLight5Lights.LightNumber.Light2, Row7StackLight);
            AddLightToMachineMap(E65, StackLight5Lights.LightNumber.Light3, Row7StackLight);

            //** Row 8
            //AddLightToMachineMap(G64, StackLight5Lights.LightNumber.Light0, Row8StackLight);
            //AddLightToMachineMap(G63, StackLight5Lights.LightNumber.Light1, Row8StackLight);
            //		AddLightToMachineMap(F63, StackLight5Lights.LightNumber.Light2, Row8StackLight);
            AddLightToMachineMap(E66, StackLight5Lights.LightNumber.Light3, Row8StackLight);

            //** Row 9
            //AddLightToMachineMap(G72, StackLight5Lights.LightNumber.Light0, Row9StackLight);
            //AddLightToMachineMap(G71, StackLight5Lights.LightNumber.Light1, Row9StackLight);
            //AddLightToMachineMap(G81, StackLight5Lights.LightNumber.Light2, Row9StackLight);
            AddLightToMachineMap(F71, StackLight5Lights.LightNumber.Light3, Row9StackLight);
            AddLightToMachineMap(F73, StackLight5Lights.LightNumber.Light4, Row9StackLight);

            //** Row 10
            //AddLightToMachineMap(F83, StackLight5Lights.LightNumber.Light0, Row10StackLight);
            AddLightToMachineMap(F82, StackLight5Lights.LightNumber.Light1, Row10StackLight);
            AddLightToMachineMap(F81, StackLight5Lights.LightNumber.Light2, Row10StackLight);
            //AddLightToMachineMap(F84, StackLight5Lights.LightNumber.Light3, Row10StackLight);

            //** Row 11
            //AddLightToMachineMap(G82, StackLight5Lights.LightNumber.Light0, Row11StackLight);
            //AddLightToMachineMap(F84, StackLight5Lights.LightNumber.Light1, Row11StackLight);

            //** Row 12
            //AddLightToMachineMap(G91, StackLight5Lights.LightNumber.Light0, Row12StackLight);
            //AddLightToMachineMap(F92, StackLight5Lights.LightNumber.Light1, Row12StackLight);




            AddEmailReport(
                toEmailAddressesForHourly: new[] { "bnewman@betzmachine.com", "MELaughlin@tramgroup.com", "Dhoward@tramgroup.com" },
                toEmailAddressesForShiftly: new[] { "bnewman@betzmachine.com", "MELaughlin@tramgroup.com", "Dhoward@tramgroup.com" },
                shift: Shift.First);
            AddEmailReport(
                toEmailAddressesForHourly: new[] { "bnewman@betzmachine.com", "MELaughlin@tramgroup.com", "Dhoward@tramgroup.com" },
                toEmailAddressesForShiftly: new[] { "bnewman@betzmachine.com", "MELaughlin@tramgroup.com", "Dhoward@tramgroup.com" },
                shift: Shift.Second);
            AddEmailReport(
                toEmailAddressesForHourly: new[] { "bnewman@betzmachine.com", "MELaughlin@tramgroup.com", "Dhoward@tramgroup.com" },
                toEmailAddressesForShiftly: new[] { "bnewman@betzmachine.com", "MELaughlin@tramgroup.com", "Dhoward@tramgroup.com" },
                shift: Shift.Third);
        }
    }
//}
