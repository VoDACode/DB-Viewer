namespace ssdb_lw_4.Attributes
{
    public class IncludeAttribute : Attribute
    {
        public string PropertyName { get; set; } = string.Empty;
        public IncludeAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
        public IncludeAttribute()
        {
        }
    }
}
