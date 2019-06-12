using CSScriptLib;
using RAL.Manager.Configuration;
using System;


namespace CS_ScriptTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // var userConfigurationTest = CSScript.Evaluator.LoadFile<IUserConfig>("UserConfigurationTest.cs");
            var userConfigurationTest = new UserConfigurationTest();
        }
    }
}
