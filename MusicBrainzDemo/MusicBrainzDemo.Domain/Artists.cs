using System;

namespace MusicBrainzDemo.Domain
{
    public class Artist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Disambiguation { get; set; }
    }
}
