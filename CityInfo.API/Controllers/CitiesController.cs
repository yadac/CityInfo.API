using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;
        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            // return Ok(CitiesDataStore.Current.Cities);
            var cities = _cityInfoRepository.GetCities();
            var result = new List<CityWithoutPointsOfInterestDto>();
            foreach (var city in cities)
            {
                result.Add(new CityWithoutPointsOfInterestDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description,
                });
            }
            return Ok(result);

        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            //return Ok(city);

            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null) {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var cityResult = new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description,
                };

                foreach (var poi in city.PointsOfInterest)
                {
                    cityResult.PointOfInterestDtos.Add(new PointOfInterestDto()
                    {
                        Id = poi.Id,
                        Name = poi.Name,
                        Description = poi.Description,
                    });
                }

                return Ok(cityResult);
            }

            // not include
            var cityWithoutPointsOfInterestDto = new CityWithoutPointsOfInterestDto()
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description,
            };

            return Ok(cityWithoutPointsOfInterestDto);

        }
    }
}
