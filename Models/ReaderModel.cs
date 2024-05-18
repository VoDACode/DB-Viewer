using ssdb_lw_4.Attributes;
using ssdb_lw_4.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ssdb_lw_4.Models
{
    [Table("reader")]
    public class ReaderModel : ITable
    {
        [Key]
        [Column("id", TypeName = "INT")]
        public int Id { get; set; }
        [Required]
        [Column("library_id", TypeName = "INT")]
        public int LibraryId { get; set; }
        [Include]
        public LibraryModel Library { get; set; }
        [Required]
        [Column("name", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Column("phone", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [Column("email", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Column("address", TypeName = "NVARCHAR")]
        [MaxLength(150)]
        public string Address { get; set; }

        [ForeignKey("ReaderId")]
        public virtual BookReaderModel BookReader { get; set; }

        public static implicit operator ReaderModel(Requests.ReaderRequest request)
        {
            return new ReaderModel
            {
                LibraryId = request.LibraryId,
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address
            };
        }
    }
}
