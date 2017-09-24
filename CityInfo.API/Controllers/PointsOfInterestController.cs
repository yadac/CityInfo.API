using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller

    {
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointOfInterestDtos);
        }

        [HttpGet("{cityId}/pointsofinterest/{pId}")]
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
    }
}
