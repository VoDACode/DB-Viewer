using ssdb_lw_4.Interfaces;
using ssdb_lw_4.Requests;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ssdb_lw_4.Models
{
    [Table("book")]
    public class BookModel : ITable
    {
        [Key]
        [Column("id", TypeName = "INT")]
        public int Id { get; set; }
        [Required]
        [Column("library_id", TypeName = "INT")]
        [ForeignKey("LibraryModel")]
        public int LibraryId { get; set; }
        public LibraryModel Library { get; set; }
        [Required]
        [Column("name", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Column("author", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Author { get; set; }
        [Required]
        [Column("year", TypeName = "INT")]
        public int Year { get; set; }

        public static implicit operator BookModel(BookRequest request)
        {
            return new BookModel
            {
                LibraryId = request.LibraryId,
                Name = request.Name,
                Author = request.Author,
                Year = request.Year
            };
        }
    }
}
