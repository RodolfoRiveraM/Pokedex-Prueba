namespace Pokemon2.Models
{
    public class TypeResponse
    {
        public List<TypeItem> Results { get; set; } = new();
    }

    public class TypeItem
    {
        public string Name { get; set; } = string.Empty;
    }
}
