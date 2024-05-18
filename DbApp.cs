using ssdb_lw_4.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ssdb_lw_4
{
    public class DbApp : IDisposable
    {
        private readonly string connectionString;
        private readonly SqlConnection connection;

        public DbApp(IConfiguration options)
        {
            connectionString = options.GetConnectionString("DB");
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
        }

        public SqlConnection GetDbConnection()
        {
            return connection;
        }

        public (DataTable, List<string>) RunSQL(string sql)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            reader.Close();
            return (table, new List<string>());
        }
        public (DataTable, List<string>) RunSQL<T>(string sql, T data)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var prop in typeof(T).GetProperties().Where(p => p.GetCustomAttribute<ColumnAttribute>() != null))
            {
                var column = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
                var value = prop.GetValue(data);
                command.Parameters.AddWithValue(column, value);
            }
            var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            reader.Close();
            return (table, new List<string>());
        }

        public IList<T> DataTableToObjects<T>(DataTable table)
        {
            IList<T> items = table.AsEnumerable().Select(row =>
            {
                var item = Activator.CreateInstance<T>();
                var itemTableName = item.GetType().GetCustomAttribute<TableAttribute>()?.Name ?? item.GetType().Name;
                foreach (var prop in typeof(T).GetProperties())
                {
                    var column = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;

                    var isInclud = prop.GetCustomAttribute<IncludeAttribute>() != null;

                    if (isInclud)
                    {
                        var foreignTableType = prop.PropertyType;
                        var foreignTableName = foreignTableType.GetCustomAttribute<TableAttribute>()?.Name ?? foreignTableType.Name;
                        var foreignInstance = Activator.CreateInstance(foreignTableType);
                        prop.SetValue(item, foreignInstance);
                        foreach (var foreignProp in foreignTableType.GetProperties())
                        {
                            var foreignColumn = foreignProp.GetCustomAttribute<ColumnAttribute>()?.Name ?? foreignProp.Name;
                            if (table.Columns.Contains($"{foreignTableName}.{foreignColumn}"))
                            {
                                foreignProp.SetValue(foreignInstance, row[$"{foreignTableName}.{foreignColumn}"]);
                            }
                        }
                    }
                    else if (table.Columns.Contains($"{itemTableName}.{column}"))
                    {
                        prop.SetValue(item, row[$"{itemTableName}.{column}"]);
                    }
                }
                return item;
            }).ToList();

            return items;
        }
    }
}
