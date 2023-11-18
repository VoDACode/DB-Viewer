using System.ComponentModel.DataAnnotations;

namespace ssdb_lw_4.Requests
{
    public class BookReaderRequest
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public int ReaderId { get; set; }
        [Required]
        public DateTime DateOfIssue { get; set; }
    }
}
