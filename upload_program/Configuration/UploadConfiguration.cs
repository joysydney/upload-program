using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upload_program.Configuration
{
    public class UploadConfiguration
    {
        public UploadConfiguration()
        {
            dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }

        public const string Upload = "Upload";
        public string SourcePath { get; set; }
        public string CompletedPath { get; set; }
        public string SummaryResult_FilePath { get; set; }
        public long CS_Destination_ID { get; set; }
        public string dateTime { get; set; }
    }
}
