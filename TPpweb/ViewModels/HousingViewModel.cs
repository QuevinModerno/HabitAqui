using TPpweb.Models;

namespace TPpweb.ViewModels
{
    public class HousingViewModel
    {
        public double AverageStars { get; set; }
        public Housing housing { get; set; } //name & total price
        public String landLordName { get; set; } //name & rating

    }
}
