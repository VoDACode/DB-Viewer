using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/bookreader")]
    [ApiController]
    public class BookReaderController : TableController<BookReaderModel, BookReaderRequest>
    {
        public BookReaderController(DbApp db) : base(db)
        {
        }

        protected override IQueryable<BookReaderModel> LoadDate(DbSet<BookReaderModel> table) => table.Include(p => p.Book).ThenInclude(p => p.Library).Include(p => p.Reader).ThenInclude(p => p.Library);

        protected override BookReaderModel ConvertToTable(BookReaderRequest request) => request;

        protected override void UpdateTable(BookReaderModel table, BookReaderRequest request)
        {
            table.ReaderId = request.ReaderId;
            table.BookId = request.BookId;
            table.DateOfIssue = request.DateOfIssue;
        }
        protected override void BeforePost(DbSet<BookReaderModel> table, ref BookReaderModel record)
        {
            var r = record;
            var reader = db.Reader.SingleOrDefault(p => p.Id == r.ReaderId);
            if (reader is null)
            {
                throw new Exception("Reader not found");
            }
            record.Reader = reader;
            var book = db.Books.SingleOrDefault(p => p.Id == r.BookId);
            if (book is null)
            {
                throw new Exception("Book not found");
            }
            record.Book = book;
        }
    }
}
