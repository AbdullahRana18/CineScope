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
        public async Task<TmdbResponse?> SearchMoviesAsync (string query)
        {
            string url;
            if (_useBearer)
            {
                // v4 token in header, call endpoint without api_key query
                url = $"{_baseUrl}/search/movie?query={Uri.EscapeDataString(query)}";
            }
            else
            {
                // v3 key expects ?api_key=KEY
                url = $"{_baseUrl}/search/movie?api_key={_apiKeyOrToken}&query={Uri.EscapeDataString(query)}";
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
        public async Task<MovieDto?> GetMovieDetailsAsync(int movieId)
        {
            string url;
            if (_useBearer)
            {
                // v4 token in header, call endpoint without api_key query
                url = $"{_baseUrl}/movie/{movieId}";
            }
            else
            {
                // v3 key expects ?api_key=KEY
                url = $"{_baseUrl}/movie/{movieId}?api_key={_apiKeyOrToken}";
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
                return JsonConvert.DeserializeObject<MovieDto>(json);
            }
            catch
            {
                // JSON parse failed
                return null;
            }
        }
        public async Task<TmdbResponse?> GetSimilarMoviesAsync(int movieId)
        {
            string url = _useBearer
                ? $"{_baseUrl}/movie/{movieId}/similar"
                : $"{_baseUrl}/movie/{movieId}/similar?api_key={_apiKeyOrToken}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TmdbResponse>(json);
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

        // ✅ new fields added below
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("runtime")]
        public int? Runtime { get; set; }

        [JsonProperty("genres")]
        public List<GenreDto> Genres { get; set; } = new();

        [JsonProperty("popularity")]
        public double Popularity { get; set; }
    }

    public class GenreDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

}
