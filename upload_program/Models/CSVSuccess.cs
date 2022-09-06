using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upload_program.Models
{
    public class CSVSuccess
    {
        public CSVSuccess(){}

        public CSVSuccess(string path) 
        {
            this.path = path;
        }

        private string _date;
        [CsvHelper.Configuration.Attributes.Name("DateTime")]
        public string date {
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
    }
}
