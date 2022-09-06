using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using upload_program.Models;

namespace upload_program.Services
{
    public interface ICSAccess
    {
        Task<CSNode> GetNode(long id);
        Task<long> CreateFolder(long parentID);
        Task<long> EnsureFolderExist(long parentID);
        Task<string> UploadFile(string path, long parentID, string dir);
    }
}
