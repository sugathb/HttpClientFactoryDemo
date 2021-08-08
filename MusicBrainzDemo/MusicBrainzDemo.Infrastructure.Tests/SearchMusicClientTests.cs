using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq.Protected;

namespace MusicBrainzDemo.Infrastructure.Tests
{
    [TestFixture]
    public class SearchMusicClientTests
    {
        private readonly Guid _artistId = Guid.Parse("bf24ca37-25f4-4e34-9aec-460b94364cfc");
        private const string ArtistName = "Shakira";
        private const string Country = "Colombia";
        private const string Disambiguation = "Colombian pop vocalist";
        private const string Gender = "Female";
        private const string ResponseContent = "{\"Artists\":[{\"Id\":\"bf24ca37-25f4-4e34-9aec-460b94364cfc\",\"Name\":\"Shakira\",\"Gender\":\"Female\",\"Country\":\"Colombia\",\"Disambiguation\":\"Colombian pop vocalist\"}]}";

        private HttpClient _httpClient;
        private Mock<ILogger<SearchMusicClient>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(ResponseContent),
                });

            _httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://samplemusicapi.org")
            };

            _mockLogger = new Mock<ILogger<SearchMusicClient>>();
        }

        [Test]
        public async Task FilterArtistsByNameAsync_Returns_Correct_Data()
        {
            var client = new SearchMusicClient(_httpClient, _mockLogger.Object);
            var response = await client.FilterArtistsByNameAsync(ArtistName);
            Assert.AreEqual(1, response.Artists.Count);
            Assert.AreEqual(_artistId, response.Artists[0].Id);
            Assert.AreEqual(ArtistName, response.Artists[0].Name);
            Assert.AreEqual(Country, response.Artists[0].Country);
            Assert.AreEqual(Disambiguation, response.Artists[0].Disambiguation);
            Assert.AreEqual(Gender, response.Artists[0].Gender);
        }
    }
}
