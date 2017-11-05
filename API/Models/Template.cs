using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Template
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string DownloadToken { get; set; }
    }
}