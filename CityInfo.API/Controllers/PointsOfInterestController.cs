using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
   [Route ("api/cities")]
   public class PointsOfInterestController : Controller
   {
      private readonly ILogger<PointsOfInterestController> _logger;
      private readonly IMailServices _mailService; // Holds the instance.
      private readonly ICityInfoRepository _cityInfoRepository;

      public PointsOfInterestController (
         ILogger<PointsOfInterestController> logger,
         IMailServices mailService,
         ICityInfoRepository cityInfoRepository)
      {
         _logger = logger;
         _mailService = mailService; // Injects into the constructor.
         _cityInfoRepository = cityInfoRepository;
      }

      // Returns Points of Interest by City Id
      [HttpGet ("{cityId}/pointsofinterest")]
      public IActionResult GetPointsOfInterest (int cityId)
      {
         try
         {
            if (!_cityInfoRepository.CityExists (cityId))
            {
               _logger.LogInformation ($"City with id {cityId} wasn't found when accessing points of interest.");
               return NotFound ();
            }

            var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity (cityId);
            var pointsOfInterestForCityResults = Mapper.Map<IEnumerable<PointOfInterest>> (pointsOfInterestForCity);
            return Ok (pointsOfInterestForCityResults);

            #region In Memory Data
            // //throw new Exception ("Exception sample");

            // var city = CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == cityId);

            // if (city == null)
            // {
            //    //_logger.LogInformation ($"City with id {cityId} wasn't found when accesing points of interest.");
            //    return NotFound ();
            // }

            // return Ok (city.PointsOfInterest);
            #endregion

            #region Without AutoMapper
            // if (!_cityInfoRepository.CityExists (cityId))
            // {
            //    _logger.LogInformation ($"City with id {cityId} wasn't found when accessing points of interest.");
            //    return NotFound ();
            // }

            // var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity (cityId);

            // var pointsOfInterestForCityResults = new List<PointOfInterest> ();

            // foreach (var poi in pointsOfInterestForCity)
            // {
            //    pointsOfInterestForCityResults.Add (new PointOfInterest ()
            //    {
            //       Id = poi.Id,
            //          Name = poi.Name,
            //          Description = poi.Description
            //    });
            // }

            // return Ok (pointsOfInterestForCityResults);
            #endregion            
         }
         catch (System.Exception)
         {
            //_logger.LogCritical ($"Exception while getting points of interest for city with id {cityId}.", ex);
            return StatusCode (500, "A problem happened while handling your request.");
         }
      }

      // Returns Points of Interest by City Id and Point of Interest Id
      [HttpGet ("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
      public IActionResult GetPointOfInterest (int cityId, int id)
      {
         if (!_cityInfoRepository.CityExists (cityId)) return NotFound ();

         var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity (cityId, id);

         if (pointOfInterest == null) return NotFound ();

         var pointOfInterestResult = Mapper.Map<PointOfInterest> (pointOfInterest);

         return Ok (pointOfInterestResult);

         #region In Memory Data
         // var city = CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == cityId);

         // if (city == null)
         // {
         //    return NotFound ();
         // }

         // var pointOfInterest = city.PointsOfInterest.FirstOrDefault (p => p.Id == id);

         // if (pointOfInterest == null) return NotFound ();

         // return Ok (pointOfInterest);
         #endregion

         #region Without AutoMapper
         // if (!_cityInfoRepository.CityExists (cityId)) return NotFound ();

         // var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity (cityId, id);

         // if (pointOfInterest == null) return NotFound ();

         // var pointOfInterestResult = new PointOfInterest ()
         // {
         //    Id = pointOfInterest.Id,
         //    Name = pointOfInterest.Name,
         //    Description = pointOfInterest.Description
         // };

         // return Ok (pointOfInterestResult);
         #endregion         
      }

      // Creates a Point of Interest by City Id
      [HttpPost ("{cityId}/pointofinterest")]
      public IActionResult CreatePointOfInterest (int cityId, [FromBody] PointOfInterestForCreation pointOfInterest)
      {
         // Body from the request
         if (pointOfInterest == null)
         {
            return BadRequest ();
         }

         if (pointOfInterest.Description == pointOfInterest.Name)
         {
            ModelState.AddModelError ("Description", "The provided description should be different from the name");
         }

         if (!ModelState.IsValid)
         {
            return BadRequest (ModelState);
         }

         if (!_cityInfoRepository.CityExists (cityId))
         {
            return NotFound ();
         }

         var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest> (pointOfInterest);

         _cityInfoRepository.AddPointOfInterestForCity (cityId, finalPointOfInterest);

         if (!_cityInfoRepository.Save ())
         {
            return StatusCode (500, "A problem happened while handling your request.");
         }

         // Maps the Point of Interest Entity back to a DTO in order to return it.
         var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterest> (finalPointOfInterest);

         return CreatedAtRoute ("GetPointOfInterest", new
            {
               cityId = cityId,
                  id = createdPointOfInterestToReturn.Id
            },
            createdPointOfInterestToReturn);

         #region Without AutoMapper
         // // Validating city existence
         // var city = CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == cityId);

         // if (city == null) return NotFound ();

         // // Demo purposes - to be improved

         // // Calculates the next Id value.
         // // Gets the City Max Point of Interest value
         // var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany (c => c.PointsOfInterest).Max (p => p.Id);

         // var finalPointOfInterest = new PointOfInterest ()
         // {
         //    Id = ++maxPointOfInterestId,
         //    Name = pointOfInterest.Name,
         //    Description = pointOfInterest.Description
         // };

         // city.PointsOfInterest.Add (finalPointOfInterest);

         // // Returns 201 Created response using the name attribute in the GET method
         // return CreatedAtRoute ("GetPointOfInterest", new
         // {
         //    cityId = cityId, id = finalPointOfInterest.Id
         // }, finalPointOfInterest);
         #endregion
      }

      // Updates a Point of Interest by a City Id and a Point of Interest Id.
      [HttpPut ("{cityId}/pointsofinterest/{id}")]
      public IActionResult UpdatePointOfInterest (int cityId, int id, [FromBody] PointOfInterestForUpdate pointOfInterest)
      {
         // Body from the request
         if (pointOfInterest == null) return BadRequest ();

         if (pointOfInterest.Description == pointOfInterest.Name) ModelState.AddModelError ("Description", "The provided description should be different from the name");

         if (!ModelState.IsValid) return BadRequest (ModelState);

         if (!_cityInfoRepository.CityExists (cityId))
         {
            return NotFound ();
         }

         var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity (cityId, id);

         if (pointOfInterest == null)
         {
            return NotFound ();
         }

         // Maps the Request Body to the Entity
         Mapper.Map (pointOfInterest, pointOfInterestEntity);

         if (!_cityInfoRepository.Save ())
         {
            return StatusCode (500, "A problem happened while handling your request");
         }

         return NoContent ();

         #region Without AutoMapper
         // // Validating City existence
         // var city = CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == cityId);

         // if (city == null) return NotFound ();

         // // Validating Point of Interest existence
         // var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault (p => p.Id == id);

         // if (pointOfInterestFromStore == null) return NotFound ();

         // // Update
         // pointOfInterestFromStore.Name = pointOfInterest.Name;
         // pointOfInterestFromStore.Description = pointOfInterest.Description;

         // // Update successful, nothing to return
         // return NoContent ();
         #endregion
      }

      // Partially updates a Point of Interest by City Id and Point of Interest Id
      [HttpPatch ("{cityId}/pointsofinterest/{id}")]
      public IActionResult PartialUpdatePointOfInterest (int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdate> patchDoc)
      {
         if (patchDoc == null)
         {
            return BadRequest ();
         }

         if (!_cityInfoRepository.CityExists (cityId))
         {
            return NotFound ();
         }

         var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity (cityId, id);
         if (pointOfInterestEntity == null)
         {
            return NotFound ();
         }

         // Maps the Request Body to the Entity
         var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdate> (pointOfInterestEntity);

         patchDoc.ApplyTo (pointOfInterestToPatch, ModelState);

         if (!ModelState.IsValid)
         {
            return BadRequest (ModelState);
         }

         if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
         {
            ModelState.AddModelError ("Description", "The provided description should be different from the name.");
         }

         TryValidateModel (pointOfInterestToPatch);

         if (!ModelState.IsValid)
         {
            return BadRequest (ModelState);
         }

         // Maps back into the Entity
         Mapper.Map (pointOfInterestToPatch, pointOfInterestEntity);

         if (!_cityInfoRepository.Save ())
         {
            return StatusCode (500, "A problem happened while handling your request.");
         }

         return NoContent ();

         #region Without AutoMapper

         // // Validating City existence
         // var city = CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == cityId);

         // if (city == null) return NotFound ();

         // // Validating Point of Interest existence
         // var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault (c => c.Id == id);

         // if (pointOfInterestFromStore == null) return NotFound ();

         // var pointOfInterestToPatch = new PointOfInterestForUpdate ()
         // {
         //    Name = pointOfInterestFromStore.Name,
         //    Description = pointOfInterestFromStore.Description
         // };

         // patchDoc.ApplyTo (pointOfInterestToPatch, ModelState);

         // if (!ModelState.IsValid) return BadRequest (ModelState);

         // if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name) ModelState.AddModelError ("Description", "The provided description should be different from the name");

         // TryValidateModel (pointOfInterestToPatch);

         // if (!ModelState.IsValid) return BadRequest (ModelState);

         // pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
         // pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

         // return NoContent ();

         #endregion
      }

      // Deletes a Point of Interest by City Id and Point of Interest Id
      [HttpDelete ("{cityId}/pointsofinterest/{id}")]
      public IActionResult DeletePointOfInterest (int cityId, int id)
      {
         if (!_cityInfoRepository.CityExists (cityId))
         {
            return NotFound ();
         }

         var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity (cityId, id);
         if (pointOfInterestEntity == null)
         {
            return NotFound ();
         }

         _cityInfoRepository.DeletePointOfInterest (pointOfInterestEntity);

         if (!_cityInfoRepository.Save ())
         {
            return StatusCode (500, "A problem happened while handling your request.");
         }

         // _mailService.Send ("Point of interest deleted.",
         //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

         return NoContent ();

         #region Without AutoMapper
         // // Validating City existence
         // var city = CitiesDataStore.Current.Cities.FirstOrDefault (c => c.Id == cityId);

         // if (city == null) return NotFound ();

         // // Validating Point of Interest existence
         // var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault (c => c.Id == id);

         // if (pointOfInterestFromStore == null) return NotFound ();

         // city.PointsOfInterest.Remove (pointOfInterestFromStore);

         // _mailService.Send ("Point of interest deleted", $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

         // return NoContent ();
         #endregion

      }

   }
}
