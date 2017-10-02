using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public string Password { get; set; }
        public List<Template> Templates { get; set; }
        public Role Role { get; set; }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendFormat("User: id: {0}, name: {1}", Id, Name);
            return s.ToString();
        }
    }
}