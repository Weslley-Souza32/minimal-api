using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
        Administrator PostAdministrator(Administrator administrator);
        List<Administrator> GetAllAdministrators(int? page);
        Administrator? GetById(int id);
    }
}