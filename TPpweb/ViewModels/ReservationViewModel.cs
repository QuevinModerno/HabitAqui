
using System.ComponentModel.DataAnnotations;


namespace TPpweb.ViewModels
{
    public class ReservationViewModel
    {
        [Display(Name = "ID da Reserva")]
        public int Id { get; set; }
        [Display(Name = "Nome Cliente", Prompt = "Introduza o nome do cliente")]
        public string Client { get; set; }
        [Display(Name = "Data de Início", Prompt = "yyyy-mm-dd")]
        public DateTime BeginDate { get; set; }
        [Display(Name = "Data de Fim", Prompt = "yyyy-mm-dd")]
        public DateTime EndDate { get; set; }
        public int FkClient { get; set; }
        public int FkHousing { get; set; }

        public bool AvaliacaoDada {  get; set; }

        [Display(Name = "Pontuação")]
        public double RatingStars { get; set; }

        [Display(Name = "Data de Avaliação")]
        [DataType(DataType.Date)]
        public DateTime RatingDate { get; set; }

        [Display(Name = "Comentário")]
        public string RatingComment { get; set; }
    }
}
