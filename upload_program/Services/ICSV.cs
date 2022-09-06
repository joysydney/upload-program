using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using upload_program.Models;

namespace upload_program.Services
{
    public interface ICSV
    {
        Task writeCSVSuccess(CSVSuccess CSVSuccess, string pathfolder);
        Task writeCSVFail(CSVFail CSVFail, string pathfolder);
    }
}
