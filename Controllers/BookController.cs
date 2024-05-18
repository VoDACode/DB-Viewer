using Microsoft.AspNetCore.Mvc;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : TableController<BookModel, BookRequest>
    {
        public BookController(DbApp db, ILogger<BookController> logger) : base(db, logger)
        {
        }


        protected override BookModel ConvertToTable(BookRequest request) => request;

        protected override void UpdateTable(BookModel table, BookRequest request)
        {
            table.Author = request.Author;
            table.Name = request.Name;
            table.Year = request.Year;
            table.LibraryId = request.LibraryId;
        }

        protected override void BeforePost(ref BookModel record)
        {
            var r = record;

            var query = $"SELECT * FROM library WHERE id = {r.LibraryId}";
            var (result, print) = db.RunSQL(query);
            var lib = db.DataTableToObjects<LibraryModel>(result).FirstOrDefault();
            if (lib is null)
            {
                throw new Exception("Library not found");
            }
            record.Library = lib;
        }
    }
}
