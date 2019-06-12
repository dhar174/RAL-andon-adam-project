using Serilog;
using System.Runtime.CompilerServices;

namespace TheColonel2688.Utilities
{
    public static class SerilogHereHelper
    {
        public static ILogger Here(
            this ILogger logger,
            string Type,
            string InstanceDescription = null,
            [CallerMemberName] string memberName = "")
        {
            if(InstanceDescription is null || InstanceDescription == "")
            {
                InstanceDescription = "not set";
            }

            return logger
                .ForContext("MemberName", memberName)
                .ForContext("Type", Type)
                .ForContext("InstanceDescription", InstanceDescription);
        }

        public static string TemplateForHere = "[{Timestamp: HH:mm: ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {Type}[{InstanceDescription}].{MemberName} {NewLine}{Exception}{NewLine}";
    }
}
