using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
   public class CityInfoRepository : ICityInfoRepository
   {
      private readonly CityInfoContext _context;

      public CityInfoRepository (CityInfoContext context)
      {
         _context = context;
      }

      // Adds a Point of Interest to a City.
      public void AddPointOfInterestForCity (int cityId, PointOfInterest pointOfInterest)
      {
         var city = GetCity (cityId, false);
         city.PointsOfInterest.Add (pointOfInterest);
      }

      // Returns True if the City exists.
      public bool CityExists (int cityId)
      {
         return _context.Cities.Any (c => c.Id == cityId);
      }

      // Returns a list of Cities ordered by Name.
      public IEnumerable<City> GetCities ()
      {
         return _context.Cities.OrderBy (c => c.Name).ToList ();
      }

      // Returns a City, with or without its Points of Interest.
      public City GetCity (int cityId, bool includePointsOfInterest)
      {
         if (includePointsOfInterest)
            return _context.Cities.Include (c => c.PointsOfInterest)
               .Where (c => c.Id == cityId).FirstOrDefault ();

         return _context.Cities
            .Where (c => c.Id == cityId).FirstOrDefault ();
      }

      // Returns a Point of Interest of a City.
      public PointOfInterest GetPointOfInterestForCity (int cityId, int pointOfInterestId)
      {
         return _context.PointsOfInterest
            .Where (p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefault ();
      }

      // Returns Points of Interest of a City.    
      public IEnumerable<PointOfInterest> GetPointsOfInterestForCity (int cityId)
      {
         return _context.PointsOfInterest
            .Where (p => p.CityId == cityId).ToList ();
      }

      // Saves the context into the database for persistence.
      public bool Save ()
      {
         // Returns amount of entities that have been changed.
         return (_context.SaveChanges () >= 0);
      }

      // Deletes a Point of Interest.
      public void DeletePointOfInterest (PointOfInterest pointOfInterest)
      {
         _context.PointsOfInterest.Remove (pointOfInterest);
      }
   }
}
