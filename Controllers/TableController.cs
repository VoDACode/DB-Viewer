using Microsoft.AspNetCore.Mvc;
using ssdb_lw_4.Attributes;
using ssdb_lw_4.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace ssdb_lw_4.Controllers
{
    public abstract class TableController<Table, Request> : ControllerBase
        where Table : class, ITable
        where Request : class
    {
        private ILogger logger;
        protected string TableName => typeof(Table).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(Table).Name;
        protected string IdField
        {
            get
            {
                var param = typeof(Table).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);
                if (param == null)
                    throw new Exception("Table must have a field with KeyAttribute");
                return param.GetCustomAttribute<ColumnAttribute>()?.Name ?? param.Name;
            }
        }
        protected Type[] ConnectedTables => typeof(Table).GetProperties().Where(p => p.GetCustomAttribute<IncludeAttribute>() != null).Select(p => p.PropertyType).ToArray();
        protected string[] Filds => typeof(Table).GetProperties().Where(p => p.GetCustomAttribute<ColumnAttribute>() != null).Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name).ToArray();
        protected string[] FildsWhithoutId => typeof(Table)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() != null && p.GetCustomAttribute<KeyAttribute>() == null)
            .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name)
            .ToArray();

        protected virtual void BeforePost(ref Table record) { }

        protected DbApp db;
        public TableController(DbApp db, ILogger<TableController<Table, Request>> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        protected abstract Table ConvertToTable(Request request);
        protected abstract void UpdateTable(Table table, Request request);

        [HttpGet]
        public virtual IActionResult Get()
        {
            string query = $"SELECT {string.Join(", ", Filds.Select(p => $"{TableName}.{p} AS [{TableName}.{p}]"))}";
            if (ConnectedTables.Length > 0)
            {
                foreach (var connectedTable in ConnectedTables)
                {
                    string connectedTableName = connectedTable.GetCustomAttribute<TableAttribute>()?.Name ?? connectedTable.Name;
                    string idField = connectedTable.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?.GetCustomAttribute<ColumnAttribute>()?.Name ?? "Id";
                    string referenceFieldName = $"{TableName}_{idField}";

                    query += $", {string.Join(", ", connectedTable.GetProperties().Where(p => p.GetCustomAttribute<ColumnAttribute>() != null).Select(p => $"{connectedTableName}.{p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name} AS [{connectedTableName}.{p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name}]"))}";
                }
                query += $" FROM {TableName} AS {TableName}";
                foreach (var connectedTable in ConnectedTables)
                {
                    string connectedTableName = connectedTable.GetCustomAttribute<TableAttribute>()?.Name ?? connectedTable.Name;
                    string idField = connectedTable.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?.GetCustomAttribute<ColumnAttribute>()?.Name ?? "Id";
                    string referenceFieldName = $"{connectedTableName}_{idField}";
                    // LEFT JOIN library AS library ON book.library_id = library.id
                    query += $" LEFT JOIN {connectedTableName} AS {connectedTableName} ON {TableName}.{referenceFieldName} = {connectedTableName}.{idField}";
                }
            }
            else
            {
                query += $" FROM {TableName} AS {TableName}";
            }

            try
            {
                var (result, print) = db.RunSQL(query);
                var records = db.DataTableToObjects<Table>(result);
                return Ok(records);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing query: {query}", query);
                return BadRequest("Something went wrong. See logs for details.");
            }
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetById([FromRoute] int id)
        {
            string query = $"SELECT {string.Join(", ", Filds)} FROM {TableName} WHERE {IdField} = {id}";
            try
            {
                var (result, print) = db.RunSQL(query);
                var record = db.DataTableToObjects<Table>(result).FirstOrDefault();
                if (record == null)
                    return NotFound();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing query: {query}", query);
                return BadRequest("Something went wrong. See logs for details.");
            }
        }

        [HttpPost]
        public virtual IActionResult Post([FromBody] Request request)
        {
            var table = ConvertToTable(request);
            string query = $"INSERT INTO {TableName} ({string.Join(", ", FildsWhithoutId)}) VALUES ({string.Join(", ", FildsWhithoutId.Select(p => $"@{p}"))});";
            query += $"SELECT {string.Join(", ", Filds)} FROM {TableName} WHERE {IdField} = SCOPE_IDENTITY();";
            try
            {
                BeforePost(ref table);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var (result, print) = db.RunSQL<Table>(query, table);
                var record = db.DataTableToObjects<Table>(result).FirstOrDefault();
                if (record == null)
                    return NotFound();
                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing query: {query}", query);
                return BadRequest("Something went wrong. See logs for details.");
            }
        }

        [HttpPut("{id}")]
        public virtual IActionResult Put([FromBody] Request request, [FromRoute] int id)
        {
            string query = $"SELECT {string.Join(", ", Filds)} FROM {TableName} WHERE {IdField} = {id}";
            try
            {
                var (result, print) = db.RunSQL(query);
                var record = db.DataTableToObjects<Table>(result).FirstOrDefault();
                if (record == null)
                    return NotFound();

                UpdateTable(record, request);

                query = $"UPDATE {TableName} SET {string.Join(", ", FildsWhithoutId.Select(p => $"{p} = @{p}"))} WHERE Id = {id};";
                query += $"SELECT {string.Join(", ", Filds)} FROM {TableName} WHERE Id = {id}";
                var (result2, print2) = db.RunSQL(query, record);
                record = db.DataTableToObjects<Table>(result2).FirstOrDefault();

                return Ok(record);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing query: {query}", query);
                return BadRequest("Something went wrong. See logs for details.");
            }
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete([FromRoute] int id)
        {
            string query = string.Empty;
            // delete item from ConnectedTables
            DeleteConnectedTables(id, typeof(Table), IdField, 0);

            query = $"DELETE FROM {TableName} WHERE {IdField} = {id};";
            try
            {
                var (result, print) = db.RunSQL(query);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing query: {query}", query);
                return BadRequest("Something went wrong. See logs for details.");
            }
        }

        private void DeleteConnectedTables(int id, Type table, string connectByFieldName, int level)
        {
            
            string tableName = table.GetCustomAttribute<TableAttribute>()?.Name ?? table.Name;
            string idField = table.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?.GetCustomAttribute<ColumnAttribute>()?.Name ?? "Id";
            string referenceFieldName = $"{tableName}_{idField}";
            string query = string.Empty;

            Console.WriteLine($"Level: {level}\tTable: {tableName}\tId: {id}");

            Type[] connectedTables = table.GetProperties().Where(p => p.GetCustomAttribute<ForeignKeyAttribute>() != null).Select(p => p.PropertyType).ToArray();
            
            foreach (var connectedTable in connectedTables)
            {
                var connectedTableName = connectedTable.GetCustomAttribute<TableAttribute>()?.Name ?? connectedTable.Name;
                // get ids of connected tables
                query = $"SELECT {idField} FROM {connectedTableName} WHERE {referenceFieldName} = {id};";
                try
                {
                    var (result, print) = db.RunSQL(query);
                    var ids = result.Select().Select(r => r[idField]).Cast<int>().ToArray();
                    foreach (var connectedId in ids)
                    {
                        DeleteConnectedTables(connectedId, connectedTable, idField, level + 1);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while executing query: {query}", query);
                    throw new Exception("Something went wrong. See logs for details.");
                }
            }

            // delete item from table
            query = $"DELETE FROM {tableName} WHERE {connectByFieldName} = {id};";
            try
            {
                var (result, print) = db.RunSQL(query);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing query: {query}", query);
                throw new Exception("Something went wrong. See logs for details.");
            }
        }
    }
}
