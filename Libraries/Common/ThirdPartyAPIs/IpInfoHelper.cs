using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.ThirdPartyAPIs
{
    public class IpInfoHelper
    {
        private string _apiToken { get; set; }

        public IpInfoHelper()
        {
            this._apiToken = Environment.GetEnvironmentVariable("IpInfoToken");
        }
        public async Task<IpInfoResponse> GetIpInfo(string ipAddress)
        {
            IpInfoResponse result = null;
            if (ipAddress == "127.0.0.1")
            {
                ipAddress = "154.21.22.214";
            }
            string url = "https://ipinfo.io/" + ipAddress + "?token=" + this._apiToken;
            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<IpInfoResponse>(await response.Content.ReadAsStringAsync());
            }
            return result;
        }
    }
}
