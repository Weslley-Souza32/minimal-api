using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly AppDbContext _context;

        public AdministratorService(AppDbContext context)
        {
            _context = context;
        }


        public Administrator? Login(LoginDTO loginDTO)
        {
            var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }

        public List<Administrator> GetAllAdministrators(int? page)
        {
            var query = _context.Administrators.AsQueryable();

            var pages = 10;
            if(page != null)
                query = query.Skip((int)(page - 1) * pages).Take(pages);

            return query.ToList();
        }

        public Administrator PostAdministrator(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();
            return administrator;
        }

        public Administrator? GetById(int id)
        {
            return _context.Administrators.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}