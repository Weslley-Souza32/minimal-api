using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _context;

        public VehicleService(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }

        public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null)
        {
            var vehicle = _context.Vehicles.AsQueryable();

            if(!string.IsNullOrEmpty(name))
            {
                vehicle = vehicle.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%%{name.ToLower()}%%"));
            }

            var pages = 10;
            if(page != null)
                vehicle = vehicle.Skip((int)(page - 1) * pages).Take(pages);

            return vehicle.ToList();
        }

        public Vehicle? GetById(int id)
        {
            var vehicle = _context.Vehicles.Where(x => x.Id == id).FirstOrDefault();
            return vehicle;
        }

        public void PostVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }
    }
}