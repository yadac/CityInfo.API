using CityInfo.API.Entities;
using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public static class CityInfoExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            if (context.Cities.Any())
            {
                return;
            }

            var cities = new List<Entities.City>()
            {
                new Entities.City()
                {
                    Name = "New York",
                    Description = "The one with that big park.",
                    PointsOfInterest = new List<PointOfInterest>(){
                        new PointOfInterest()
                        {
                            Name = "Central Park.",
                            Description = "Central Park is an urban park in Manhattan, New York City."
                        },
                        new PointOfInterest()
                        {
                            Name = "Empire State Building.",
                            Description = "an American cultural icon."
                        }
                    }
                },
                new Entities.City()
                {
                    Name = "Tokyo",
                    Description = "2010 olympic dome.",
                    PointsOfInterest = new List<PointOfInterest>(){
                        new PointOfInterest()
                        {
                            Name = "Tokyo Tower.",
                            Description = "it is the second-tallest structure in Japan."
                        },
                        new PointOfInterest()
                        {
                            Name = "Tokyo Imperial Palace.",
                            Description = "the primary residence of the Emperor of Japan."
                        }
                    }
                },
                new Entities.City()
                {
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsOfInterest = new List<PointOfInterest>(){
                        new PointOfInterest()
                        {
                            Name = "Eiffel Tower.",
                            Description = "most-visited paid monument in the world."
                        },
                        new PointOfInterest()
                        {
                            Name = "Arc de Triomphe.",
                            Description = "one of the most famous monuments in Paris."
                        }
                    }
                },

            };

            context.Cities.AddRange(cities);
            context.SaveChanges();

        }
    }
}
