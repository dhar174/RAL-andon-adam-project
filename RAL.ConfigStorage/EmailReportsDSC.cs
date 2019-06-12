
namespace RAL.ConfigStorageTypes
{
    public class EmailReportsDSC
    {
        public string SMTPServerHostName { get; set; }

        public int SMTPServerPort { get; set; }

        public (string UserName, string Password)? Credentials { get; set; }

        public EmailReportDSC[] listOfReportConfigs { get; set; }
    }
}