using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using upload_program.Configuration;
using upload_program.Models;

namespace upload_program.Services
{
    public class CSAccess : ICSAccess
    {
        private readonly BaseConfiguration _config;
        private readonly IAuthenticate _auth;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogManagerCustom _log;
        public CSAccess(IOptions<BaseConfiguration> config, IHttpClientFactory httpClientFactory, IAuthenticate auth, ILogManagerCustom log)
        {
            _httpClientFactory = httpClientFactory;
            _config = config.Value;
            _auth = auth;
            _log = log;
        }

        public async Task<CSNode> GetNode(long id)
        { 
            string ticket = await _auth.Auth();
            var url = _config.Destination_URL + "api/v1/nodes/" + id;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("OTCSTicket", ticket);

            var httpClientFactory = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClientFactory.SendAsync(httpRequestMessage);
            try
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var temp = await httpResponseMessage.Content.ReadAsStringAsync();
                    CSNode? cSNode = JsonConvert.DeserializeObject<CSNode>(temp);
                    return cSNode;
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
            return new CSNode();
        } 

        public async Task<long> CreateFolder(long parentID)
        {
            string ticket = await _auth.Auth();
            long id = -1;
            try
            {
                var client = new RestClient("localhost/otcs/llisapi.dll/api/v1/nodes/"); 
                var request = new RestRequest("", Method.Post);
                request.AddHeader("OTCSTicket", ticket);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("type", "0");
                request.AddParameter("parent_id", parentID);
                request.AddParameter("name", "test_folder");
                var response = await client.ExecuteAsync(request);
                var content = response.Content;

                if (content != null)
                {
                    dynamic dynamicResult = JObject.Parse(content);
                    id = dynamicResult.id;
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }

            return id;
        } 

        public async Task<long> EnsureFolderExist(long parentID)
        {
            long id = -1;
            CSNode cSNode = new CSNode(); 
            try
            {
                cSNode = await GetNode(parentID);
                id = cSNode.data.id;
                if (cSNode == null)
                {
                    id = await CreateFolder(parentID);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            } 

            return id;
        }

        public async Task<string> UploadFile(string path, long parentID, string dir)
        {
            string result = "";
            try
            {
                string token = await _auth.Auth();
                string url = _config.Destination_URL + "api/v1/nodes";
                var client = new RestClient(url);
                var request = new RestRequest("", Method.Post);
                request.AddHeader("OTCSTicket", token);
                request.AddParameter("parent_id", parentID);
                request.AddParameter("type", "144");
                request.AddParameter("name", Path.GetFileName(path));
                request.AddFile("file", path);
                var response = await client.ExecuteAsync(request);
                var content = response.Content;

                if (content != null)
                {
                    dynamic dynamicResult = JObject.Parse(content);
                    result = dynamicResult.id;
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
            return result;
        }
    }
}
