using System.ComponentModel.DataAnnotations;

namespace ssdb_lw_4.Requests
{
    public class LibraryRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Address { get; set; }
        [Required]
        [MaxLength(50)]
        [Phone]
        public string Phone { get; set; }
    }
}
