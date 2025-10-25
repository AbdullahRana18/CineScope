using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CineScope.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKeyOrToken;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";
        private readonly bool _useBearer;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKeyOrToken = configuration["TMDb:ApiKey"] ?? string.Empty;

            // Detect if provided value looks like a v4 Bearer token (JWT starting with eyJ)
            _useBearer = _apiKeyOrToken.StartsWith("eyJ", StringComparison.OrdinalIgnoreCase);

            if (_useBearer)
            {
                // set Authorization header for every request (v4 token)
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKeyOrToken);
            }
        }

        public async Task<TmdbResponse?> GetTrendingMoviesAsync()
        {
            string url;
            if (_useBearer)
            {
                // v4 token in header, call endpoint without api_key query
                url = $"{_baseUrl}/trending/movie/week";
            }
            else
            {
                // v3 key expects ?api_key=KEY
                url = $"{_baseUrl}/trending/movie/week?api_key={_apiKeyOrToken}";
            }

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                // Optionally log status code here
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<TmdbResponse>(json);
            }
            catch
            {
                // JSON parse failed
                return null;
            }
        }
    }

    public class TmdbResponse
    {
        [JsonProperty("results")]
        public List<MovieDto> Results { get; set; } = new();
    }

    public class MovieDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; } = string.Empty;
    }
}
