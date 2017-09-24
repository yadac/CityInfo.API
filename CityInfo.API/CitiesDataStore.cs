using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            // init dummy data
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York",
                    Description = "The one with that big park.",
                    PointOfInterestDtos = new List<PointOfInterestDto>(){
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park.",
                            Description = "Central Park is an urban park in Manhattan, New York City."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State Building.",
                            Description = "an American cultural icon."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Tokyo",
                    Description = "2010 olympic dome.",
                    PointOfInterestDtos = new List<PointOfInterestDto>(){
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Tokyo Tower.",
                            Description = "it is the second-tallest structure in Japan."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Tokyo Imperial Palace.",
                            Description = "the primary residence of the Emperor of Japan."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointOfInterestDtos = new List<PointOfInterestDto>(){
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Eiffel Tower.",
                            Description = "most-visited paid monument in the world."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Arc de Triomphe.",
                            Description = "one of the most famous monuments in Paris."
                        }
                    }
                },

            };

        }
    }
}
