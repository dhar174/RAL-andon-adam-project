namespace RAL.Manager.Configuration
{
    public class EmailServerConfiguration
    {
        public string EmailServer { get; set; } // = "smtp.ipower.com";

        public int EmailServerPort { get; set; } // = 587;

        //public string FromEmailAddress { get; set; } // = "RAL-System-noreply@tramgroup.com";

        public (string UserName, string Password)? Credentials { get; set; } // = ("betz@betzmachine.com", "Betz-320");
    }
}