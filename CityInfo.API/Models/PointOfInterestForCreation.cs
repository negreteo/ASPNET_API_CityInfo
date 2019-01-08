using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
   public class PointOfInterestForCreation
   {
      [Required (ErrorMessage = "You must provide a name value")]
      [MaxLength (50)]
      public string Name { get; set; }

      [MaxLength (200)]
      public string Description { get; set; }
   }
}
