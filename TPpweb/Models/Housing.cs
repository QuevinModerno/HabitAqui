using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TPpweb.Models
{
    public class Housing
    {
        //comentario
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Accomodation Name")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Max 250 chars min 3")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Available From")]
        public DateTime AvailableFrom { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Available Until")]
        public DateTime AvailableUntil { get; set; }

        [Required]
        [Display(Name = "Location")]
        [StringLength(5000, MinimumLength = 5, ErrorMessage = "Max 5000 chars min 5")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Propertie value")]

        public int TotalPrice { get; set; }

        [Required]
       
        [ForeignKey("LandLord")]
        [Display(Name = "Landlord")]
        public int FkLandLord { get; set; }
        public Landlord Landlord { get; set; }

        [Display(Name = "Image")]
        public string? ImagePath { get; set; }

        [NotMapped] 
        public IFormFile? FotoFile { get; set; }

        public int Area { get; set; }

        public int NBedroom { get; set; }
        public int Nbathroom { get; set; }

        public string TipeHabit { get; set; }
        public int NPeople { get; set; }
       
        public int PriceMonth { get; set; }
        public string Description { get; set; }

        public List<Rating> Ratings { get; set; }

    }
}
