
using Microsoft.Extensions.Options;
using upload_program.Configuration;
using upload_program.Utils;

namespace upload_program.Services
{
    public class SecureInfo
    {
        private readonly BaseConfiguration _config;
        public SecureInfo(IOptions<BaseConfiguration> config)
        {
            _config = config.Value;
        }

        public string getSensitiveInfo(string secureFileName)
        {
            var fileName = _config.secureInfo.fileName;

            string credentialsDirectory = _config.secureInfo.Path;
            string AESKeyFilePath = Path.Combine(credentialsDirectory, fileName.AESKey);
            string secureFilePath = Path.Combine(credentialsDirectory, secureFileName);

            return CryptographyCore.ReadSensitiveData(secureFilePath, AESKeyFilePath);
            return "";
        }
    }
}
