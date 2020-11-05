using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cosette.Tuner.Common.Requests;
using Newtonsoft.Json;

namespace Cosette.Tuner.Web
{
    public class WebService
    {
        private HttpClient _httpClient;
        private bool _enabled;

        public WebService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:42000/api/");
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 1);
        }

        public async Task EnableIfAvailable()
        {
            try
            {
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "ping"));
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[{DateTime.Now}] Web client enabled!");
                    _enabled = true;
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}] Web client disabled");
            }
        }

        public async Task SendGenerationData(GenerationDataRequest requestData)
        {
            if (!_enabled)
            {
                return;
            }

            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("generation", httpContent);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException();
                }
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}] Request containing generation data failed!");
            }
        }

        public async Task SendChromosomeData(ChromosomeDataRequest requestData)
        {
            if (!_enabled)
            {
                return;
            }

            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chromosome", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException();
                }
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}] Request containing chromosome data failed!");
            }
        }
    }
}
