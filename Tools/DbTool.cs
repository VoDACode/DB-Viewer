using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ssdb_lw_4.Models.SQL;
using System.Data;

namespace ssdb_lw_4.Tools
{
    public static class DbTool
    {
        public static List<Models.SQL.SqlParameter> GetParameters(this DbApp db, string entity)
        {
            List<Models.SQL.SqlParameter> result = new List<Models.SQL.SqlParameter>();
            var conn = db.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = $"SELECT ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM information_schema.parameters WHERE SPECIFIC_NAME = '{entity}';";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var obj = new Models.SQL.SqlParameter();
                obj.Position = reader.GetInt32(0);
                var mode = reader.GetString(1);
                if (mode.Equals("IN"))
                    obj.Mode = ParameterMode.IN;
                else if (mode.Equals("OUT"))
                    obj.Mode = ParameterMode.OUT;
                else if (mode.Equals("INOUT"))
                    obj.Mode = ParameterMode.INOUT;
                obj.IsResult = reader.GetValue(2).Equals("YES");
                obj.Name = reader.GetString(3);
                obj.Type = reader.GetString(4);
                obj.CharacterMaxLenght = reader.IsDBNull(5) ? null : reader.GetInt32(5);

                result.Add(obj);
            }

            return result;
        }

        public static List<string> GetFunctions(this DbApp db) => GetRoutines(db, "FUNCTION");

        public static List<string> GetProcedures(this DbApp db) => GetRoutines(db, "PROCEDURE");

        public static (DataTable, List<string>) CallProcedure(this DbApp db, string name, List<SqlCallParameter> arguments)
        {
            var conn = db.Database.GetDbConnection() as SqlConnection;
            if (conn.State != ConnectionState.Open)
                conn.Open();
            var command = conn.CreateCommand();

            List<string> printMesaages = new List<string>();

            conn.InfoMessage += (s, e) =>
            {
                printMesaages.Add(e.Message);
            };

            DataTable resultTable = new DataTable($"{name}");

            List<SqlCallParameter> outputParams = arguments.Where(a => a.Mode != ParameterMode.IN).ToList();
            if (outputParams.Count > 0)
            {
                command.CommandText = $"DECLARE ";
                foreach (var arg in outputParams)
                {
                    command.CommandText += $"@{arg.Name}_out {arg.Type}, ";
                }
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2);
                command.CommandText += ";";
            }

            command.CommandText += $"EXEC {name} ";
            foreach (var arg in arguments)
            {
                if (arg.Mode != ParameterMode.IN)
                {
                    command.CommandText += $"{arg.Name} = @{arg.Name}_out OUTPUT, ";
                }
                else if (arg.Type == "varchar" || arg.Type == "nvarchar" || arg.Type == "date")
                {
                    command.CommandText += $"{arg.Name} = N'{arg.Value}', ";
                }
                else
                {
                    command.CommandText += $"{arg.Name} = {arg.Value}, ";
                }
            }
            if (arguments.Count > 0)
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2);
            command.CommandText += ";";

            if (outputParams.Count > 0)
            {
                command.CommandText += $"SELECT";
                foreach (var arg in outputParams)
                {
                    command.CommandText += $" @{arg.Name}_out AS [{arg.Name.Remove(0, 1)}],";
                }

                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 1);
                command.CommandText += ";";
            }

            using (var reader = command.ExecuteReader())
            {
                resultTable.Load(reader);
            }

            return (resultTable, printMesaages);
        }

        public static (DataTable, List<string>) CallFunction(this DbApp db, string name, List<SqlCallParameter> arguments)
        {
            var conn = db.Database.GetDbConnection() as SqlConnection;
            if (conn.State != ConnectionState.Open)
                conn.Open();
            var command = conn.CreateCommand();

            List<string> printMesaages = new List<string>();

            conn.InfoMessage += (s, e) =>
            {
                printMesaages.Add(e.Message);
            };


            DataTable resultTable = new DataTable($"{name}");

            command.CommandText += $"SELECT {(arguments.Any(p => p.Mode != ParameterMode.IN) ? "" : "* FROM")} dbo.{name}(";
            foreach (var arg in arguments)
            {
                if (arg.Mode == ParameterMode.IN)
                {
                    if (arg.Type == "varchar" || arg.Type == "nvarchar" || arg.Type == "date")
                    {
                        command.CommandText += $"N'{arg.Value}', ";
                    }
                    else
                    {
                        command.CommandText += $"{arg.Value}, ";
                    }
                }
            }
            if (arguments.Count > 0)
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2);
            command.CommandText += ");";

            using (var reader = command.ExecuteReader())
            {
                resultTable.Load(reader);
            }

            return (resultTable, printMesaages);
        }

        private static List<string> GetRoutines(DbApp db, string routingType)
        {
            List<string> result = new List<string>();

            var conn = db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = $"SELECT SPECIFIC_NAME FROM information_schema.routines WHERE ROUTINE_TYPE = '{routingType}'";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }
            return result;
        }
    }
}
