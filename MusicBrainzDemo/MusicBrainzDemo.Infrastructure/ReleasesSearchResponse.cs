using System;
using System.Collections.Generic;
using MusicBrainzDemo.Domain;

namespace MusicBrainzDemo.Infrastructure
{
    public class ReleasesSearchResponse
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Disambiguation { get; set; }
        public IList<Release> Releases { get; set; } = new List<Release>();
    }
}
