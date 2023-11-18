using Microsoft.EntityFrameworkCore;
using ssdb_lw_4.Models;

namespace ssdb_lw_4
{
    public class DbApp : DbContext
    {
        public DbSet<LibraryModel> Library { get; set; }
        public DbSet<BookModel> Books { get; set; }
        public DbSet<ReaderModel> Reader { get; set; }
        public DbSet<BookReaderModel> BooksReader { get; set; }

        public DbApp(DbContextOptions<DbApp> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
