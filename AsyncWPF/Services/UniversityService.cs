using AsyncWPF.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace AsyncWPF.Services
{
    internal class UniversityService
    {
        private readonly HttpClient _client;
        private const string BASE_URL = "https://localhost:7299/countries/";

        public UniversityService()
        {
            _client = new();
            _client.BaseAddress = new Uri(BASE_URL);
        }

        public async Task<List<University>> GetByCountryName(string countryName, CancellationToken token)
        {
            var response = await _client.GetAsync(BASE_URL + countryName, token);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<University>>(json)
                ?? throw new JsonSerializationException($"Error deserializing universities data.");

            return data.Take(5).ToList();
        }
    }
}
