using System.ComponentModel.DataAnnotations;

namespace MusicBrainzDemo.Api.V1.Models
{
    public class SearchMusicRequest
    {
        [Required(ErrorMessage = "Artist name cannot be are empty.")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "Please enter a valid artist name.")]
        public string ArtistName { get; set; }
    }
}
