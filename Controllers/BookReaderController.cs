using Microsoft.AspNetCore.Mvc;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/bookreader")]
    [ApiController]
    public class BookReaderController : TableController<BookReaderModel, BookReaderRequest>
    {
        public BookReaderController(DbApp db, ILogger<BookReaderController> logger) : base(db, logger)
        {
        }

        protected override BookReaderModel ConvertToTable(BookReaderRequest request) => request;

        protected override void UpdateTable(BookReaderModel table, BookReaderRequest request)
        {
            table.ReaderId = request.ReaderId;
            table.BookId = request.BookId;
            table.DateOfIssue = request.DateOfIssue;
        }
        protected override void BeforePost(ref BookReaderModel record)
        {
            var r = record;

            string query = $"SELECT * FROM book_reader WHERE id = {r.ReaderId}";
            var (dataTableReader, _) = db.RunSQL(query);
            if (dataTableReader.Rows.Count == 0)
            {
                throw new Exception("Reader not found");
            }

            query = $"SELECT * FROM book WHERE id = {r.BookId}";
            var (dataTableBook, _) = db.RunSQL(query);
            if (dataTableBook.Rows.Count == 0)
            {
                throw new Exception("Book not found");
            }

            var reader = db.DataTableToObjects<ReaderModel>(dataTableReader).First();
            record.Reader = reader;
            var book = db.DataTableToObjects<BookModel>(dataTableBook).First();
            record.Book = book;
        }
    }
}
