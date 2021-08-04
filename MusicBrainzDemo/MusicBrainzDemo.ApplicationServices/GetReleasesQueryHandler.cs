using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MusicBrainzDemo.Infrastructure;

namespace MusicBrainzDemo.ApplicationServices
{
    public class GetReleasesQuery : IRequest<ReleasesSearchResponse>
    {
        public GetReleasesQuery(Guid artistId)
        {
            ArtistId = artistId;
        }

        public Guid ArtistId { get; }
    }

    public class GetReleasesQueryHandler : IRequestHandler<GetReleasesQuery, ReleasesSearchResponse>
    {
        private readonly ISearchMusicClient _searchMusicClient;

        public GetReleasesQueryHandler(ISearchMusicClient artistsSearchClient)
        {
            _searchMusicClient = artistsSearchClient;
        }

        public async Task<ReleasesSearchResponse> Handle(GetReleasesQuery request, CancellationToken cancellationToken)
        {
            return await _searchMusicClient.FilterReleasesByArtistIdAsync(request.ArtistId).ConfigureAwait(false);
        }
    }
}
