using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using upload_program.Configuration;
using upload_program.Services;

namespace upload_program
{
    public class Authenticate : IAuthenticate
    {
        private readonly BaseConfiguration _config;
        private readonly ILogManagerCustom _log;
        public Authenticate(IOptions<BaseConfiguration> config, ILogManagerCustom log)
        {
            _config = config.Value;
            _log = log;
        }

        public async Task<string> Auth()
        {
            string authToken = "";
            bool success = false;
            int attemptCount = 1;
            int noOfAttempts = 2;
            while (success == false && attemptCount <= noOfAttempts)
            {
                try
                {
                    string OTCS_loginId = "", OTCS_password = "";
                    if (_config.UseSecureCredentials)
                    {
                        OTCS_loginId = SecureInfo.getSensitiveInfo(_config.secureInfo.fileName.CSUsername);
                        OTCS_password = SecureInfo.getSensitiveInfo(_config.secureInfo.fileName.CSPassword);
                    }
                    else
                    {
                        OTCS_loginId = _config.Username;
                        OTCS_password = _config.Password;
                    }

                    string url = _config.Destination_URL + "api/v1/auth";
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                    Dictionary<string, string> formData = new Dictionary<string, string>();
                    formData.Add("username", OTCS_loginId);
                    formData.Add("password", OTCS_password);

                    httpRequestMessage.Content = new FormUrlEncodedContent(formData);
                    HttpClient httpClient = new HttpClient();
                    var response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
                    try
                    {
                        string res = await response.Content.ReadAsStringAsync();
                        dynamic dynamicResult = JObject.Parse(res);
                        authToken = dynamicResult.ticket;
                        success = true;
                    }
                    catch (Exception e)
                    {
                        _log.debug(e.Message.ToString());
                    }
                }
                catch (Exception e)
                {
                    _log.debug("failed to get token: attempt " + attemptCount + " of " + noOfAttempts);

                    if (attemptCount >= noOfAttempts)
                    {
                        throw e;
                    }
                    attemptCount++;
                }
            }
            return authToken;
        }
    }
}
