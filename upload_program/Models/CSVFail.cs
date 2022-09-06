using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upload_program.Models
{
    public class CSVFail
    { 
        public CSVFail() { }
        public CSVFail(string path, string errorCode, string recAction)
        { 
            this.path = path;
            this.errorCode = errorCode;
            this.recAction = recAction; 
        } 

        private string _date;
        [CsvHelper.Configuration.Attributes.Name("DateTime")]
        public string date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }
        }

        [CsvHelper.Configuration.Attributes.Name("Path")]
        public string path { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Error Code")]
        public string errorCode { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Recommended Action")]
        public string recAction { get; set; } 
    }
}
