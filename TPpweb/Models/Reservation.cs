using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TPpweb.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdReservation { get; set; }

        [ForeignKey("Housing")]
        public int FkHousing { get; set; }
        public virtual Housing Housing { get; set; }


        [ForeignKey("Client")]
        public string FkClient { get; set; }
        public virtual Person Client { get; set; }

        [Required]
        [Display(Name = "Data de ínicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataInicio { get; set; }

        [Required]
        [Display(Name = "Data de fim")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataFim { get; set; }

        [Required]
        [Display(Name = "Price")]
        public float Price { get; set; }

        public bool accepted { get; set; }

        [Display(Name = "Avaliação Dada")]
        public bool AvaliacaoDada { get; set; }

        public virtual DeliveryDetails? Delivery { get; set; }
        public virtual DeliveryDetails? Return { get; set; }
    }
}
