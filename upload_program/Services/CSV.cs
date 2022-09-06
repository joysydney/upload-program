using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using upload_program.Models;

namespace upload_program.Services
{
    public class CSV : ICSV
    {
        public async Task writeCSVSuccess(CSVSuccess CSVSuccess, string path)
        {  
            var tableCSVRows = new List<CSVSuccess>
                                {
                                    new CSVSuccess {
                                        date = CSVSuccess.date,
                                        path = CSVSuccess.path
                                    }
                                };
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(path)
            };
            using (var writer = new StreamWriter(path, true))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                await csv.WriteRecordsAsync(tableCSVRows);
            }
        }
        public async Task writeCSVFail(CSVFail CSVFail, string path)
        { 
            var tableCSVRows = new List<CSVFail>
                                {
                                    new CSVFail {
                                        date = CSVFail.date,
                                        path = CSVFail.path,
                                        errorCode = CSVFail.errorCode,
                                        recAction = CSVFail.recAction
                                    }
                                };
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(path)
            };
            using (var writer = new StreamWriter(path, true))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                await csv.WriteRecordsAsync(tableCSVRows);
            }
        }
    }
}
