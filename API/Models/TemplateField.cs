using Newtonsoft.Json;

namespace API.Models
{
    public class TemplateField
    {
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
        [JsonProperty(PropertyName = "replacement")]
        public string Replacement { get; set; }

        public new string ToString()
        {
            return $"TemplateField: Content = \"{Content}\", Comment = \"{Comment}\", Replacement = \"{Replacement}\"";
        }
    }
}