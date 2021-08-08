using System;
using System.Threading.Tasks;

namespace MusicBrainzDemo.Infrastructure
{
    public interface ISearchMusicClient
    {
        Task<ArtistSearchResponse> FilterArtistsByNameAsync(string artistName);
    }
}
