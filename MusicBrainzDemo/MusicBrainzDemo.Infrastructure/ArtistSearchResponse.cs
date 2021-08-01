using System.Collections.Generic;
using MusicBrainzDemo.Domain;

namespace MusicBrainzDemo.Infrastructure
{
    public class ArtistSearchResponse
    {
        public IList<Artist> Artists { get; set; } = new List<Artist>();
    }
}
