using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using upload_program.Configuration;
using upload_program.Models;
using upload_program.Services;

namespace upload_program
{
    public class Main :IMain
    {
        private readonly UploadConfiguration _uploadConfig;
        private readonly ICSAccess _csAccess;
        private readonly ICSV _csv;
        private string datename;
        private string filename;
        SemaphoreSlim sm = new SemaphoreSlim(1);
        public Main(IOptions<UploadConfiguration> uploadConfig, ICSAccess csAccess, ICSV csv)
        {
            _uploadConfig = uploadConfig.Value;
            _csAccess = csAccess;
            _csv = csv;
            datename = DateTime.Now.ToString("yyyyMMdd_HHmmss"); 
            filename = Path.GetFullPath(_uploadConfig.SummaryResult_FilePath) + "Result_" + datename;
        }
         
        public async Task ProcessFiles()
        { 
            string source_path = _uploadConfig.SourcePath;
            string completed_path = _uploadConfig.CompletedPath;
            long ID = _uploadConfig.CS_Destination_ID;
            long totalProcessed = 0;
            long totalUploaded = 0;
            long totalFailUpload = 0;

            var tasks = new List<Task>();
            var paths = Directory.EnumerateFiles(source_path, "*", SearchOption.AllDirectories);
            foreach (var path in paths)
            {
                totalProcessed++;
                long Dest_NodeID = ID;
                string t = path.Substring(source_path.Length, path.Length - source_path.Length);

                string[] filepathArr = t.Split('\\');

                string fileName = Path.GetFileName(path); 

                string containerName = ""; 
                string destPath = "", dir = Path.Combine(completed_path, fileName);
                int j = 0, k = filepathArr.Length;
                if (k > 1)
                {
                    while (j < k)
                    {  
                        if (j != filepathArr.Length - 1)
                        {
                            containerName = filepathArr[j];
                            Dest_NodeID = await _csAccess.EnsureFolderExist(Dest_NodeID); 
                            destPath += filepathArr[j] + '\\';
                            dir = Path.Combine(completed_path, destPath);
                            bool exist = System.IO.Directory.Exists(dir);
                            if (!exist)
                                System.IO.Directory.CreateDirectory(dir);
                        }
                        else
                        {
                            dir = Path.Combine(completed_path, Path.Combine(destPath, fileName));
                        }
                        j++;
                    }
                }
                tasks.Add(ProcessFile(path, ID, dir, totalUploaded, totalFailUpload));
            }
            await Task.WhenAll(tasks);
            await WriteSummary(totalProcessed, totalFailUpload, totalUploaded);
        }

        public async Task ProcessFile(string path, long Dest_NodeID, string dir, long totalUploaded, long totalFailUpload)
        {
            if (!Directory.Exists(filename))
            {
                Directory.CreateDirectory(filename);
            }
            try
            {
                var result = await _csAccess.UploadFile(path, Dest_NodeID, dir);
                totalUploaded++;
                string temp = "SUCCESS_" + datename + ".csv";
                string name = Path.Combine(filename, temp);
                await sm.WaitAsync();
                string destPath = Path.Combine(_uploadConfig.CompletedPath, dir);
                await MoveFile(path, destPath);
                await _csv.writeCSVSuccess(new CSVSuccess(path), name);
                sm.Release();
            }
            catch(Exception ex)
            {
                totalFailUpload++;
                string temp = "FAILED_" + datename + ".csv";
                string name = Path.Combine(filename, temp);
                await sm.WaitAsync();
                await _csv.writeCSVFail(new CSVFail(path, ex.Message.ToString(), ""), name);
                sm.Release();
            }
        } 

        private async Task MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(destinationFile))
                    {
                        await sourceStream.CopyToAsync(destinationStream); 
                        sourceStream.Close();
                        File.Delete(sourceFile);
                    }
                }
            } 
            catch (Exception ex)
            {
                
            }
        }
        public async Task WriteSummary(long totalProcessed, long totalFailUpload, long totalUploaded)
        {
            string txtSummary = Path.Combine(filename, "SUMMARY_" + _uploadConfig.dateTime + ".txt");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamWriter sw = new StreamWriter(txtSummary, true, System.Text.Encoding.GetEncoding(1252)))
            {
                await sw.WriteLineAsync(DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss") + "\n");
                await sw.WriteLineAsync("Total items processed: " + totalProcessed);
                await sw.WriteLineAsync("Total items failed to upload: " + totalFailUpload);
                await sw.WriteLineAsync("Total items uploaded: " + totalUploaded); 
            } 
        }
    }
}
