using System.ComponentModel.DataAnnotations;

namespace API.BindingModels
{
    public class LoginUserBindingModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
