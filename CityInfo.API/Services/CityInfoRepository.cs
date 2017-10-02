using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                // include related data(point of interest) or not.
                // this is because of we get data from sql-database, not memory-database.
                // memory-database always includes related data too.
                // So, we distinguish get it or not by adding boolean data.
                return _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefault();
            }

            // return only city data.
            return _context.Cities.Where(c => c.Id == cityId).FirstOrDefault();

        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pId)
        {
            return _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointOfInterests.Where(p => p.CityId == cityId).ToList();
        }

        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest poi)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(poi);

        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
