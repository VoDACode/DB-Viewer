using System.ComponentModel.DataAnnotations;

namespace ssdb_lw_4.Requests
{
    public class BookRequest
    {
        [Required]
        public int LibraryId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Author { get; set; }
        [Required]
        public int Year { get; set; }
    }
}
