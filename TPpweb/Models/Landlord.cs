using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TPpweb.Models
{
    public class Landlord
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLandlord { get; set; }

        [Required(ErrorMessage = "Introduza o seu nome")]
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string ContactInformation { get; set; }

        public bool active { get; set; }

        public List<Housing> Properties { get; set; }
        public List<Rating> ratings { get; set; }

    }
}
