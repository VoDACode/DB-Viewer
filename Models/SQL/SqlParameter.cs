using System.Text.Json.Serialization;

namespace ssdb_lw_4.Models.SQL
{
    public enum ParameterMode
    {
        IN,
        OUT,
        INOUT
    }

    public class SqlParameter
    {
        public int Position { get; set; }
        // serialize to string
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ParameterMode Mode { get; set; }
        public bool IsResult { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? CharacterMaxLenght { get; set; }
    }
}
