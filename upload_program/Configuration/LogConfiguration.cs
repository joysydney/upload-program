namespace upload_program.Services
{
    public class LogConfiguration
    {
        public const string Logger = "Logger";
        public string LogDirectory { get; set; }
        public string LogFileName_Main { get; set; }
        public string LogFileLevel { get; set; }
        public string LogConsoleLevel { get; set; }
    }
}