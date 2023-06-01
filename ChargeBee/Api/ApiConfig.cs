using System;
using System.Net.Http;
using System.Text;
using ChargeBee.Internal;
using Newtonsoft.Json.Linq;

namespace ChargeBee.Api
{
    public sealed class ApiConfig
    {
        public readonly HttpClient HttpClient;
        
        public static string DomainSuffix = "chargebee.com";
		public static string Proto = "https";
		public static string Version = "3.5.0";
		public static readonly string API_VERSION = "v2";
        public static int TimeTravelMillis { get; set; }
        public static int ExportSleepMillis { get; set;}

        public string ApiKey { get; set; }
        public string SiteName { get; set; }
        public string Charset { get; set; }
        public static int ConnectTimeout { get; set; }

        public string ApiBaseUrl
        {
            get
            {
				return String.Format("{0}://{1}.{2}/api/{3}",
                    Proto,
                    SiteName,
                    DomainSuffix,
					API_VERSION);
            }
        }

        public string AuthValue
        {
            get
            {
                return String.Format("Basic {0}",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    String.Format("{0}:", ApiKey))));
            }
        }

        public ApiConfig(string siteName, string apiKey, HttpClient httpClient)
        {
            if (String.IsNullOrEmpty(siteName))
                throw new ArgumentException("Site name can't be empty!");

            if (String.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Api key can't be empty!");
            
            //
            // (MM) Updated client factory for use in API requests...NOPE.
            // Gotta be the client itself at this stage.
            HttpClient = httpClient;
            
            //
            // Suitable httpClient for repeated usage here or in the utilities class??
            // If here, then convert to private or protected then call .CreateClient()

            Charset = Encoding.UTF8.WebName;
            ConnectTimeout = 30000;
            TimeTravelMillis = 3000;
            ExportSleepMillis = 10000;
            SiteName = siteName;
            ApiKey = apiKey;
        }

        private static volatile ApiConfig m_instance;

        public static void Configure(string siteName, string apiKey, HttpClient httpClient)
        {         
            m_instance = new ApiConfig(siteName, apiKey, httpClient);
        }

        public static string SerializeObject<T>(T t)where T : Resource
        {
            return t.GetJToken().ToString();
        }

        public static T DeserializeObject<T>(string str)where T : Resource, new()
        {
            JToken JObj = JToken.Parse(str);	
            T t = new T();
            t.JObj = JObj;
            return t;
        }

        public static ApiConfig Instance
        {
            get
            {
                if (m_instance == null)
                    throw new Exception("Not yet configured!");

                return m_instance;
            }
        }

        public static void updateConnectTimeoutInMillis(int timeout) {
                    ConnectTimeout = timeout;
        }
    }
}
