using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upload_program.Configuration
{
    public class BaseConfiguration
    {
        public const string APIConnection = "APIConnection";
        public string Host { get; set; }
        public string Home { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Destination_URL { get; set; }
        public bool UseSecureCredentials { get; set; }    
        public Secure secureInfo { get; set; }
    }

    public class Secure
    {
        public string Path { get; set; }
        public FileName fileName { get; set; }
    } 

    public class FileName
    {  
        public string AESKey { get; set; }
        public string CSUsername { get; set; }
        public string CSPassword { get; set; }
        public string CSDBUsername { get; set; }
        public string CSDBPassword { get; set; }
    }
}
