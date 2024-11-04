using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TPpweb.Models
{
    public class Employer
    {
        [Key]
        public int Id { get; set; }

        [Required]

        [ForeignKey("LandLord")]
        [Display(Name = "Landlord")]
        public int LandlordId { get; set; }
        public Landlord Landlord { get; set; }

        [Required]

        [ForeignKey("Person")]
        [Display(Name = "Person")]
        public string PersonId { get; set; }
        public Person Person { get; set; }

    }
}
