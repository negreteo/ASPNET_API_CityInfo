using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
   //[Route ("api/[controller]")] or
   [Route ("api/cities")]
   public class CitiesController : Controller
   {
      private readonly ICityInfoRepository _citiInfoRepository;

      public CitiesController (ICityInfoRepository citiInfoRepository)
      {
         _citiInfoRepository = citiInfoRepository;
      }

      #region JSON format

      // // Returns cities in JSON format
      // [HttpGet ()]
      // public JsonResult GetCities_JSON ()
      // {
      //    // // Returning JSON from anonymous objects
      //    // return new JsonResult (new List<object> ()
      //    // {
      //    //    new { id = 1, Name = "New York City" },
      //    //    new { id = 2, Name = "Antwerp" }
      //    // });

      //    // Returning JSON from the data store
      //    return new JsonResult (CitiesDataStore.Current.Cities);
      // }

      // // Returns a city by Id in JSON format
      // [HttpGet ("{id}")]
      // public JsonResult GetCity_JSON (int id)
      // {
      //    return new JsonResult (CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == id));
      // }

      #endregion

      // Returns Cities
      [HttpGet ()]
      public IActionResult GetCities ()
      {
         var citiesEntities = _citiInfoRepository.GetCities ();

         var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterest>> (citiesEntities);

         return Ok (results);

         #region In Memory Data
         //return Ok (CitiesDataStore.Current.Cities);
         #endregion

         #region Without AutoMapper
         // var citiesEntities = _citiInfoRepository.GetCities ();
         // var results = new List<CityWithoutPointsOfInterest> ();

         // foreach (var cityEntity in citiesEntities)
         // {
         //    results.Add (new CityWithoutPointsOfInterest
         //    {
         //       Id = cityEntity.Id,
         //          Name = cityEntity.Name,
         //          Description = cityEntity.Description

         //    });
         // }

         // return Ok (results);
         #endregion         
      }

      // Returns a City by Id 
      [HttpGet ("{id}")]
      public IActionResult GetCity (int id, bool includePointsOfInterest = false)
      {
         var city = _citiInfoRepository.GetCity (id, includePointsOfInterest);

         if (city == null) return NotFound ();

         // Returns city with point of interest
         if (includePointsOfInterest)
         {
            var cityResult = Mapper.Map<City> (city);
            return Ok (cityResult);
         }

         // Returns city without point of interest
         var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterest> (city);
         return Ok (cityWithoutPointsOfInterestResult);

         #region In Memory Data
         // var city = (CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == id));

         // if (city == null) return NotFound ();
         // return Ok (city);
         #endregion         
      }

   }
}
