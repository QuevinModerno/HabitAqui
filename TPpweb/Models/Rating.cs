using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TPpweb.Models
{
    public class Rating
    {
        [Key]
        public int IdAvaliacao { get; set; }

        [ForeignKey("Reservation")]
        public int FkReservation { get; set; }
        public virtual Reservation Reservation { get; set; }


        [Range(0.0, 5.0, ErrorMessage = "Avaliação entre 0 e 5.")]
        public double Stars { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateRating { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }
    }
}
