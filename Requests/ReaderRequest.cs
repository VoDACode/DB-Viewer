using System.ComponentModel.DataAnnotations;

namespace ssdb_lw_4.Requests
{
    public class ReaderRequest
    {
        [Required]
        public int LibraryId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(150)]
        public string Address { get; set; }
    }
}
