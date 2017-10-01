using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public string Password { get; set; }
//        public List<Template> Templates { get; set; }
//
//        [Required]
//        public Role Role { get; set; }
    }
}