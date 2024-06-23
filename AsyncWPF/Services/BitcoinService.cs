using AsyncWPF.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace AsyncWPF.Services
{
    internal class BitcoinService
    {
        private readonly HttpClient _client;
        private const string BASE_URL = "https://api.coindesk.com/v1/bpi/currentprice.json";

        public BitcoinService()
        {
            _client = new();
            _client.BaseAddress = new Uri(BASE_URL);
        }

        public async Task<Bitcoin> GetLatest(CancellationToken token)
        {
            var response = await _client.GetAsync("", token);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Bitcoin>(json);

            return result;
        }
    }
}
