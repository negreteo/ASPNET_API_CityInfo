using System.Collections.Generic;

namespace CityInfo.API.Models
{
   public class City
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }

      public int NumberOfPointsOfInterest
      {
         get
         {
            return PointsOfInterest.Count;
         }
      }

      public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest> ();
   }
}
