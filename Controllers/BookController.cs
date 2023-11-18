using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : TableController<BookModel, BookRequest>
    {
        public BookController(DbApp db) : base(db)
        {
        }

        protected override IQueryable<BookModel> LoadDate(DbSet<BookModel> table) => table.Include(p => p.Library);

        protected override BookModel ConvertToTable(BookRequest request) => request;

        protected override void UpdateTable(BookModel table, BookRequest request)
        {
            table.Author = request.Author;
            table.Name = request.Name;
            table.Year = request.Year;
            table.LibraryId = request.LibraryId;
        }

        protected override void BeforePost(DbSet<BookModel> table, ref BookModel record)
        {
            var r = record;
            var lib = db.Library.SingleOrDefault(p => p.Id == r.LibraryId);
            if (lib is null)
            {
                throw new Exception("Library not found");
            }
            record.Library = lib;
        }

        [HttpGet("{id}/readers")]
        public async Task<IActionResult> GetReaders(int id)
        {
            var readers = await db.BooksReader
                                    .Include(p => p.Reader)
                                    .Where(p => p.BookId == id)
                                    .Select(p => p.Reader)
                                    .ToListAsync();

            return Ok(readers);
        }
    }
}
