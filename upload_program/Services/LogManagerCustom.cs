using Microsoft.Extensions.Options;
using NLog;
using NLog.Targets;

namespace upload_program.Services
{
    public class LogManagerCustom : ILogManagerCustom
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string mainLogFileFullName = "";
        private static string currentLogFileFullName = "";

        private readonly LogConfiguration loggerConfig;
        public LogManagerCustom(IOptions<LogConfiguration> app)
        {
            loggerConfig = app.Value;
            InitializeLogger();
        }
        public void InitializeLogger()
        {
            string LogDirectory = loggerConfig.LogDirectory;
            string LogFileName_Main = loggerConfig.LogFileName_Main;
            string LogFileLevel = loggerConfig.LogFileLevel;
            string LogConsoleLevel = loggerConfig.LogConsoleLevel;

            string LogFilePath = getNewLogFullPath(LogDirectory, LogFileName_Main);

            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = LogFilePath,
                Layout = "${date:format=yyyyMMdd-HHmmss}|${level}|${callsite}|${message}"
            };
            var logconsole = new ConsoleTarget("logconsole");

            config.AddRule(LogLevel.FromString(LogConsoleLevel), LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.FromString(LogFileLevel), LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
            currentLogFileFullName = LogFilePath;
            mainLogFileFullName = LogFilePath;
        }
        private static string getNewLogFullPath(string logDirectory, string logName)
        {
            string logExtension = ".txt";
            string logFilePath = Path.Combine(logDirectory, logName);
            return logFilePath + "_" + System.DateTime.Now.ToString("yyyyMMdd-HHmmss") + logExtension;
        }
        public void debug(string message)
        {
            logger.Debug(message);
        }
        public void info(string message)
        {
            logger.Info(message);
        }
    }
}
