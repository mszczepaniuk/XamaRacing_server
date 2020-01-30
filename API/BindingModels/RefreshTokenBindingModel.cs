using System.ComponentModel.DataAnnotations;

namespace API.BindingModels
{
    public class RefreshTokenBindingModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
