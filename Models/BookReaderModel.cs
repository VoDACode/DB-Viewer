using ssdb_lw_4.Attributes;
using ssdb_lw_4.Interfaces;
using ssdb_lw_4.Requests;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ssdb_lw_4.Models
{
    [Table("book_reader")]
    public class BookReaderModel : ITable
    {
        [Key]
        [Column("id", TypeName = "INT")]
        public int Id { get; set; }
        [Required]
        [Column("book_id", TypeName = "INT")]
        public int BookId { get; set; }
        [Required]
        [Column("reader_id", TypeName = "INT")]
        public int ReaderId { get; set; }
        [Include]
        public BookModel Book { get; set; }
        [Include]
        public ReaderModel Reader { get; set; }

        [Required]
        [Column("date_of_issue", TypeName = "DATE")]
        public DateTime DateOfIssue { get; set; }

        public static implicit operator BookReaderModel(BookReaderRequest request)
        {
            return new BookReaderModel
            {
                ReaderId = request.ReaderId,
                BookId = request.BookId,
                DateOfIssue = request.DateOfIssue
            };
        }
    }
}
