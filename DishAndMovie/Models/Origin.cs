using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class Origin
    {
        [Key]
        public int OriginId { get; set; }

        public string OriginCountry { get; set; }

    }
}
