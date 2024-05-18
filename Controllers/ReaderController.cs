using Microsoft.AspNetCore.Mvc;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/reader")]
    [ApiController]
    public class ReaderController : TableController<ReaderModel, ReaderRequest>
    {
        public ReaderController(DbApp db, ILogger<ReaderController> logger) : base(db, logger)
        {
        }

        protected override ReaderModel ConvertToTable(ReaderRequest request) => request;

        protected override void UpdateTable(ReaderModel table, ReaderRequest request)
        {
            table.Email = request.Email;
            table.Name = request.Name;
            table.Phone = request.Phone;
            table.Address = request.Address;
            table.LibraryId = request.LibraryId;
        }

        protected override void BeforePost(ref ReaderModel record)
        {
            var r = record;

            string query = $"SELECT * FROM library WHERE id = {r.LibraryId}";
            var (table, _) = db.RunSQL(query);
            if (table.Rows.Count == 0)
            {
                throw new Exception("Library not found");
            }

            record.Library = db.DataTableToObjects<LibraryModel>(table).First();
        }
    }
}
