using TPpweb.Models;

namespace TPpweb.ViewModels
{
    public class SearchProperties
    {
        public List<Housing> Properties { get; set; }
        public Double cost { get; set; }
        public Landlord Landlord { get; set; }


    }
}
