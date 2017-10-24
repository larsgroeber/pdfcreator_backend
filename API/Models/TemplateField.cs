namespace API.Models
{
    public class TemplateField
    {
        public string Name { get; set; }
        public string Comment { get; set; }

        public new string ToString()
        {
            return $"TemplateField: Name = \"{Name}\", Comment = \"{Comment}\"";
        }
    }
}