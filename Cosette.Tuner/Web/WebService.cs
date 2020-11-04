using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

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
                    Console.WriteLine($"[{DateTime.Now}] Web client disabled");
                }
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now}] Web client disabled");
            }
        }
    }
}
