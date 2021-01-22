using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Common.Responses;
using Newtonsoft.Json;

namespace Cosette.Tuner.Common.Services
{
    public class WebService
    {
        private HttpClient _httpClient;
        private bool _enabled;

        public WebService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:42000/api/");
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 5);
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

        public async Task<int> RegisterTest(RegisterTestRequest requestData)
        {
            if (!_enabled)
            {
                return -1;
            }

            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("test/register", httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<TestDataResponse>(responseContent).Id;
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}] Can't register new test");
            }

            return -1;
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
