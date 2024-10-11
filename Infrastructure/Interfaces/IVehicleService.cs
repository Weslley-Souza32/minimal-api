using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> GetAllVehicles(int page = 1, string? name = null, string? brand = null);
        Vehicle? GetById(int id);
        void PostVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(Vehicle vehicle);
    }
}