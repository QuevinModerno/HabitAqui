using TPpweb.ViewModels;
namespace TPpweb.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        public bool IsAdmin { get; set; }

        public bool HasAssociatedHouse { get; set; }
    }
}
