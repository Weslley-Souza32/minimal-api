using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
    }
}