﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MusicBrainzDemo.Infrastructure
{
    public class SearchMusicClient : ISearchMusicClient
    {
        private const string SearchArtistsByNameError = "Error occurred when searching artists.";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SearchMusicClient> _logger;

        public SearchMusicClient(IHttpClientFactory clientFactory, ILogger<SearchMusicClient> logger)
        {
            _httpClientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<ArtistSearchResponse> FilterArtistsByNameAsync(string artistName)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"artist?query=artist:{artistName}");
                var client = _httpClientFactory.CreateClient("musicApiClient");
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"{SearchArtistsByNameError} ArtistName: {artistName}");
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
