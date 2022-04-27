using Core.Entities;

namespace Entities.Dtos
{
    public class UserCompanyDtoForList : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime UserAddedAt { get; set; }
        public bool UserIsActive { get; set; }
        public string UserMailValue { get; set; }
    }
}