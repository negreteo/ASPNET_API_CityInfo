using System.Collections.Generic;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
   public interface ICityInfoRepository
   {
      void AddPointOfInterestForCity (int cityId, PointOfInterest pointOfInterest);
      bool CityExists (int cityId);
      IEnumerable<City> GetCities ();
      City GetCity (int cityId, bool includePointsOfInterest);
      IEnumerable<PointOfInterest> GetPointsOfInterestForCity (int cityId);
      PointOfInterest GetPointOfInterestForCity (int cityId, int PointOfInterest);
      bool Save ();
      void DeletePointOfInterest (PointOfInterest pointOfInterest);
   }
}
