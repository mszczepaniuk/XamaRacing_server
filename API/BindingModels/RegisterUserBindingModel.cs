using System.ComponentModel.DataAnnotations;

namespace API.BindingModels
{
    public class RegisterUserBindingModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
