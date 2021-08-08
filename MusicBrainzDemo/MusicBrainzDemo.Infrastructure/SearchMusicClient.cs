using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MusicBrainzDemo.Infrastructure
{
    public class SearchMusicClient : ISearchMusicClient
    {
        private const string SearchArtistsByNameError = "Error occurred when searching artists.";

        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SearchMusicClient> _logger;

        public SearchMusicClient(HttpClient httpClient, ILogger<SearchMusicClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ArtistSearchResponse> FilterArtistsByNameAsync(string artistName)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"artist?query=artist:{artistName}");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var statusCode = response.StatusCode;
                    throw new HttpRequestException($"{SearchArtistsByNameError} ArtistName: {artistName}, StatusCode: {statusCode}, ErrorMessage: {errorMessage}");
                }

                var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ArtistSearchResponse>(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{SearchArtistsByNameError} ArtistName: {{ArtistName}}", artistName);
                throw;
            }
        }
    }
}
