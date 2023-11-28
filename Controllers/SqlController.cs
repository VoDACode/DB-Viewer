using Microsoft.AspNetCore.Mvc;
using ssdb_lw_4.Models;
using ssdb_lw_4.Models.SQL;
using ssdb_lw_4.Tools;
using System.Buffers;
using System.Text;

namespace ssdb_lw_4.Controllers
{
    [Route("api/sql")]
    [ApiController]
    public class SqlController : ControllerBase
    {
        protected readonly DbApp db;

        public SqlController(DbApp db)
        {
            this.db = db;
        }

        [HttpGet("func")]
        public IActionResult GetFunc()
        {
            return Ok(db.GetFunctions());
        }

        [HttpGet("proc")]
        public IActionResult Get()
        {
            return Ok(db.GetProcedures());
        }

        [HttpGet("params/{entity}")]
        public IActionResult GetProcParams(string entity)
        {
            return Ok(db.GetParameters(entity));
        }

        [HttpGet("proc/{proc}/call")]
        public void CallProc(string proc)
        {
            var clientArgs = HttpContext.Request.Query;
            var procArgs = db.GetParameters(proc);

            List<SqlCallParameter> args = procArgs.Select(p => new SqlCallParameter()
            {
                Name = p.Name,
                IsResult = p.IsResult,
                Mode = p.Mode,
                Position = p.Position,
                Type = p.Type,
                CharacterMaxLenght = p.CharacterMaxLenght,
            }).ToList();

            foreach (var arg in procArgs)
            {
                if (arg.Mode == ParameterMode.OUT || arg.IsResult || (!clientArgs.ContainsKey(arg.Name) && arg.Mode == ParameterMode.INOUT))
                {
                    continue;
                }

                if (!clientArgs.ContainsKey(arg.Name) && arg.Mode != ParameterMode.INOUT)
                {
                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} not set"));
                    return;
                }

                var value = clientArgs[arg.Name].ToString();
                if (arg.Type == "int")
                {
                    if (!int.TryParse(value, out int intValue))
                    {
                        Response.ContentType = "text/plain";
                        Response.StatusCode = 400;
                        Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} must be int"));
                        return;
                    }
                    args.Single(p => p.Name == arg.Name).Value = intValue;
                }
                else if (arg.Type == "varchar" || arg.Type == "nvarchar")
                {
                    args.Single(p => p.Name == arg.Name).Value = value;
                }
                else if (arg.Type == "date")
                {
                    var d = DateTime.Parse(value);
                    args.Single(p => p.Name == arg.Name).Value = d.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (arg.Type == "decimal")
                {
                    if (!decimal.TryParse(value, out decimal convertedValue))
                    {
                        Response.ContentType = "text/plain";
                        Response.StatusCode = 400;
                        Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} must be decimal"));
                        return;
                    }
                    args.Single(p => p.Name == arg.Name).Value = convertedValue;
                }
                else
                {
                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} has unknown type {arg.Type}"));
                    return;
                }
            }

            var result = db.CallProcedure(proc, args);

            Response.StatusCode = 200;
            Response.ContentType = "application/xml";
            result.WriteXml(Response.BodyWriter.AsStream());
            Response.Body.Close();
        }

        [HttpGet("func/{func}/call")]
        public void CallFunction(string func)
        {
            var clientArgs = HttpContext.Request.Query;
            var funcArgs = db.GetParameters(func);

            List<SqlCallParameter> args = funcArgs.Select(p => new SqlCallParameter()
            {
                Name = p.Name,
                IsResult = p.IsResult,
                Mode = p.Mode,
                Position = p.Position,
                Type = p.Type,
                CharacterMaxLenght = p.CharacterMaxLenght,
            }).ToList();

            foreach (var arg in funcArgs)
            {
                if (arg.Mode != ParameterMode.IN)
                {
                    continue;
                }

                if (!clientArgs.ContainsKey(arg.Name) && arg.Mode != ParameterMode.INOUT)
                {
                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} not set"));
                    return;
                }

                var value = clientArgs[arg.Name].ToString();
                if (arg.Type == "int")
                {
                    if (!int.TryParse(value, out int convertedValue))
                    {
                        Response.ContentType = "text/plain";
                        Response.StatusCode = 400;
                        Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} must be int"));
                        return;
                    }
                    args.Single(p => p.Name == arg.Name).Value = convertedValue;
                }
                else if (arg.Type == "varchar" || arg.Type == "nvarchar")
                {
                    args.Single(p => p.Name == arg.Name).Value = value;
                }
                else if (arg.Type == "date")
                {
                    var d = DateTime.Parse(value);
                    args.Single(p => p.Name == arg.Name).Value = d.ToString();
                }
                else if (arg.Type == "decimal")
                {
                    if (!decimal.TryParse(value, out decimal convertedValue))
                    {
                        Response.ContentType = "text/plain";
                        Response.StatusCode = 400;
                        Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} must be decimal"));
                        return;
                    }
                    args.Single(p => p.Name == arg.Name).Value = convertedValue;
                }
                else
                {
                    Response.ContentType = "text/plain";
                    Response.StatusCode = 400;
                    Response.BodyWriter.Write(Encoding.UTF8.GetBytes($"Argument {arg.Name} has unknown type {arg.Type}"));
                    return;
                }
            }

            var result = db.CallFunction(func, args);
            Response.StatusCode = 200;
            Response.ContentType = "application/xml";
            result.WriteXml(Response.BodyWriter.AsStream());
            Response.Body.Close();
        }
    }
}
