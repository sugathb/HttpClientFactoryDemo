using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MusicBrainzDemo.Infrastructure;

namespace MusicBrainzDemo.ApplicationServices
{
    public class GetArtistsQuery : IRequest<ArtistSearchResponse>
    {
        public GetArtistsQuery(string artistName)
        {
            ArtistName = artistName;
        }

        public string ArtistName { get; }
    }

    public class GetArtistsQueryHandler : IRequestHandler<GetArtistsQuery, ArtistSearchResponse>
    {
        private const string InvalidArtistNameErrorMessage = "Invalid artist name.";

        private readonly ISearchMusicClient _searchMusicClient;

        public GetArtistsQueryHandler(ISearchMusicClient artistsSearchClient)
        {
            _searchMusicClient = artistsSearchClient;
        }

        public async Task<ArtistSearchResponse> Handle(GetArtistsQuery request, CancellationToken cancellationToken)
        {
            if (!Regex.Match(request.ArtistName, "^[A-Z][a-zA-Z]*$").Success)
            {
                throw new ArgumentException(InvalidArtistNameErrorMessage);
            }

            return await _searchMusicClient.FilterArtistsByNameAsync(request.ArtistName).ConfigureAwait(false);
        }
    }
}
