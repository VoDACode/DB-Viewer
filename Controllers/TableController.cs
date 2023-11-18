using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssdb_lw_4.Interfaces;

namespace ssdb_lw_4.Controllers
{
    public abstract class TableController<Table, Request> : ControllerBase
        where Table : class, ITable
        where Request : class
    {

        protected DbApp db;
        public TableController(DbApp db)
        {
            this.db = db;
        }

        protected virtual IQueryable<Table> LoadDate(DbSet<Table> table) => table;
        protected virtual void BeforePost(DbSet<Table> table, ref Table record) { }
        protected abstract Table ConvertToTable(Request request);
        protected abstract void UpdateTable(Table table, Request request);

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await LoadDate(db.Set<Table>()).ToListAsync());
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await LoadDate(db.Set<Table>()).SingleOrDefaultAsync(x => x.Id == id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] Request request)
        {
            var table = ConvertToTable(request);

            try
            {
                BeforePost(db.Set<Table>(), ref table);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var newEntity = (await db.Set<Table>().AddAsync(table)).Entity;

            await db.SaveChangesAsync();

            return Ok(newEntity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Request request, [FromRoute] int id)
        {
            var record = await LoadDate(db.Set<Table>()).SingleOrDefaultAsync(p => p.Id == id);

            if (record == null)
                return NotFound();

            UpdateTable(record, request);

            await db.SaveChangesAsync();

            return Ok(record);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] int id)
        {
            var record = await LoadDate(db.Set<Table>()).SingleOrDefaultAsync(p => p.Id == id);

            if (record == null)
                return NotFound();

            db.Remove(record);

            await db.SaveChangesAsync();

            return Ok(record);
        }
    }
}
