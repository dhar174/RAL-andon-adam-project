using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheColonel2688.Utilities
{
    public class ObjectDifferenceWriter
    {
        public static void DumpToFile<TExpected, TActual>(TExpected expected, TActual actual, string path)
        {


            using (StreamWriter file = File.CreateText($"{path}-Expected.json"))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, expected);
            }

            using (StreamWriter file = File.CreateText($"{path}-Actual.json"))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, actual);
            }
        }
    }
}
