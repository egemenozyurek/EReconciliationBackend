using Core.Entities;

namespace Entities.Dtos
{
    public class UserForRegisterToSecondAccountDto : UserForRegister, IDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int AdminUserId { get; set; }
    }
}