using System.ComponentModel.DataAnnotations;

namespace API.BindingModels
{
    public class RaceResultBindingModel
    {
        [Required]
        public int? RaceId { get; set; }
        [Required]
        public string Time { get; set; }
    }
}
