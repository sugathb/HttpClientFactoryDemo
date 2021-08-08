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
        private readonly ISearchMusicClient _searchMusicClient;

        public GetArtistsQueryHandler(ISearchMusicClient searchMusicClient)
        {
            _searchMusicClient = searchMusicClient;
        }

        public async Task<ArtistSearchResponse> Handle(GetArtistsQuery request, CancellationToken cancellationToken)
        {
            return await _searchMusicClient.FilterArtistsByNameAsync(request.ArtistName).ConfigureAwait(false);
        }
    }
}
