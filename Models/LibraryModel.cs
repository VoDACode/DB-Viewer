using ssdb_lw_4.Interfaces;
using ssdb_lw_4.Requests;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ssdb_lw_4.Models
{
    [Table("library")]
    public class LibraryModel : ITable
    {
        [Key]
        [Column("id", TypeName = "INT")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Column("address", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Address { get; set; }
        [Required]
        [Column("phone", TypeName = "NVARCHAR")]
        [MaxLength(50)]
        [Phone]
        public string Phone { get; set; }

        public static implicit operator LibraryModel(LibraryRequest v)
        {
            return new LibraryModel
            {
                Name = v.Name,
                Address = v.Address,
                Phone = v.Phone
            };
        }
    }
}
