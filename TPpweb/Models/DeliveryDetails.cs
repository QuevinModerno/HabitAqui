using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TPpweb.Models
{
    public class DeliveryDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string OptionalEquipment { get; set; }
        public string? Damages { get; set; }
        public string? DamageImages { get; set; }
        public string? Observations { get; set; }

        [ForeignKey("Employer")]

        public int FkEmployer { get; set; }
        public virtual Employer Employer { get; set; }


        [ForeignKey("Reservation")]

        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
