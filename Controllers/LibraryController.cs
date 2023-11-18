using Microsoft.AspNetCore.Mvc;
using ssdb_lw_4.Models;
using ssdb_lw_4.Requests;

namespace ssdb_lw_4.Controllers
{
    [Route("api/library")]
    [ApiController]
    public class LibraryController : TableController<LibraryModel, LibraryRequest>
    {
        public LibraryController(DbApp db) : base(db)
        {
        }

        protected override LibraryModel ConvertToTable(LibraryRequest request) => request;

        protected override void UpdateTable(LibraryModel table, LibraryRequest request)
        {
            table.Name = request.Name;
            table.Address = request.Address;
            table.Phone = request.Phone;
        }
    }
}
