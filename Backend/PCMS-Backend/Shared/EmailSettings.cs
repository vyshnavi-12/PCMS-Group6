namespace PCMS_Backend.Shared
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = default!;
        public int Port { get; set; }
        public string SenderName { get; set; } = default!;
        public string SenderEmail { get; set; } = default!;

        public string AuthEmail { get; set; } = default!;
        public string AppPassword { get; set; } = default!;
    }
}
