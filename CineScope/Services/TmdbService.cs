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

            // Detect if the provided token is a v4 Bearer token (starts with "eyJ")
            _useBearer = _apiKeyOrToken.StartsWith("eyJ", StringComparison.OrdinalIgnoreCase);

            if (_useBearer)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKeyOrToken);
            }
        }

        // Get trending movies
        public async Task<TmdbResponse?> GetTrendingMoviesAsync()
        {
            string url = _useBearer
                ? $"{_baseUrl}/trending/movie/week"
                : $"{_baseUrl}/trending/movie/week?api_key={_apiKeyOrToken}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TmdbResponse>(json);
        }

        // Search movies by title
        public async Task<TmdbResponse?> SearchMoviesAsync(string query)
        {
            string url = _useBearer
                ? $"{_baseUrl}/search/movie?query={Uri.EscapeDataString(query)}"
                : $"{_baseUrl}/search/movie?api_key={_apiKeyOrToken}&query={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TmdbResponse>(json);
        }

        // Get movie details
        public async Task<MovieDto?> GetMovieDetailsAsync(int movieId)
        {
            string url = _useBearer
                ? $"{_baseUrl}/movie/{movieId}"
                : $"{_baseUrl}/movie/{movieId}?api_key={_apiKeyOrToken}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MovieDto>(json);
        }

        // Get similar movies
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

        // Get top billed cast (first 8 actors)
        public async Task<List<CastInfo>?> GetMovieCastAsync(int movieId)
        {
            string url = _useBearer
                ? $"{_baseUrl}/movie/{movieId}/credits"
                : $"{_baseUrl}/movie/{movieId}/credits?api_key={_apiKeyOrToken}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var credits = JsonConvert.DeserializeObject<CreditsResponse>(json);

            return credits?.Cast?.Take(8).ToList();
        }
    }

    // DTO for TMDB API responses
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

    // Integrated Cast Models
    public class CastInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("character")]
        public string Character { get; set; } = string.Empty;

        [JsonProperty("profile_path")]
        public string? ProfilePath { get; set; }
    }

    public class CreditsResponse
    {
        [JsonProperty("cast")]
        public List<CastInfo>? Cast { get; set; }
    }
}
