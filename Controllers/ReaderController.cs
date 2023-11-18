using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/reader")]
    [ApiController]
    public class ReaderController : TableController<ReaderModel, ReaderRequest>
    {
        public ReaderController(DbApp db) : base(db)
        {
        }

        protected override IQueryable<ReaderModel> LoadDate(DbSet<ReaderModel> table) => table.Include(p => p.Library);

        protected override ReaderModel ConvertToTable(ReaderRequest request) => request;

        protected override void UpdateTable(ReaderModel table, ReaderRequest request)
        {
            table.Email = request.Email;
            table.Name = request.Name;
            table.Phone = request.Phone;
            table.Address = request.Address;
            table.LibraryId = request.LibraryId;
        }

        protected override void BeforePost(DbSet<ReaderModel> table, ref ReaderModel record)
        {
            var r = record;
            var lib = db.Library.SingleOrDefault(p => p.Id == r.LibraryId);
            if (lib is null)
            {
                throw new Exception("Library not found");
            }
            record.Library = lib;
        }

        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetBooks(int id)
        {
            var readers = await db.BooksReader
                                    .Include(p => p.Book)
                                    .Where(p => p.ReaderId == id)
                                    .Select(p => p.Book)
                                    .ToListAsync();

            return Ok(readers);
        }
    }
}
