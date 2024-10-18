using minimal_api.Domain.Enums;

namespace minimal_api.Domain.ViewsModal
{
    public record AdministratorViewsModel
    {
        public int Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Profile { get; set; } = default!;
    }
}