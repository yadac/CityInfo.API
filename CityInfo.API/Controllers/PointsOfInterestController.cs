using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository
            )
        {
            // di = not create instance.
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        // argument name has to be same value with that is in url pattern {}. 
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                var cityExists = _cityInfoRepository.CityExists(cityId);
                if (!cityExists)
                {
                    _logger.LogInformation($"city with id {cityId} was not found when accesing point of interest.");
                    return NotFound();
                }
                var pointsOfInterest = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var result = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);
                return Ok(result);

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
            var cityExists = _cityInfoRepository.CityExists(cityId);

            if (!cityExists)
            {
                _logger.LogInformation($"city with id {cityId} was not found when accesing point of interest.");
                return NotFound();
            }
            var poi = _cityInfoRepository.GetPointOfInterestForCity(cityId, pId);

            if (poi == null)
            {
                _logger.LogInformation($"city with id {cityId} and poi with id {pId} was not found when accesing point of interest.");
                return NotFound();
            }

            var result = Mapper.Map<PointOfInterestDto>(poi);

            return Ok(result);
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
                CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

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

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pId);
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

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pId);
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

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send(
                "Point of interest",
                $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

            return NoContent();
        }
    }
}
