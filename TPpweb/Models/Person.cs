using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TPpweb.Models
{
    public class Person : IdentityUser
    {
        public string? Nome { get; set; }
        public string? Morada { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int? Contacto { get; set; }
        public string? BI { get; set; } = "DefaultBIValue";
        public string? NIF { get; set; }
        public string? Passe { get; set; }
    }
}