using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            // di = not create instance.
            _logger = logger;
        }
         
        // argument name has to be same value with that is in url pattern {}. 
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                throw new Exception("sample exception");

                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"city with id {cityId} was not found when accesing point of interest.");
                    return NotFound();
                }

                return Ok(city.PointOfInterestDtos);

            }
            catch (Exception e)
            {
                _logger.LogCritical($"*** exception raise when accesing point of interest by {cityId} ***");
                return StatusCode(500, "server error");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{pId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int pId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointOfInterestDtos.FirstOrDefault(p => p.Id == pId);
            if (poi == null)
            {
                return NotFound();
            }

            return Ok(poi);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The provided Description value should be different from a name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId =
                CitiesDataStore.Current.Cities.SelectMany(c => c.PointOfInterestDtos).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };

            city.PointOfInterestDtos.Add(finalPointOfInterest);

            // like get behavior?
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pId = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPost("{cityId}/pointsofinterest/{pId}")]
        public IActionResult UpdatePointOfInterest(int cityId, int pId, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The provided Description value should be different from a name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterestDtos.FirstOrDefault(p => p.Id == pId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            // update
            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{pId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int pId, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterestDtos.FirstOrDefault(p => p.Id == pId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            // this check against input model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // business error
            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "The provided ...");
            }

            // + model error 
            TryValidateModel(pointOfInterestToPatch);

            // check against update model dto.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // update
            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();

        }

        [HttpDelete("{cityId}/pointsofinterest/{pId}")]
        public IActionResult DeletePointOfInterest(int cityId, int pId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterestDtos.FirstOrDefault(p => p.Id == pId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointOfInterestDtos.Remove(pointOfInterestFromStore);

            return NoContent();
        }
    }
}
