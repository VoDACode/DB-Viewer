using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ssdb_lw_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        protected readonly DbApp db;
        public TestController(DbApp db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetLibs()
        {
            return Ok(await db.Library.ToListAsync());
        }
    }
}
