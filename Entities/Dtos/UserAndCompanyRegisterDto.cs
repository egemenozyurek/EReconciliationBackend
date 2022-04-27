using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
    public class UserAndCompanyRegisterDto : IDto
    {
        public UserForRegister UserForRegister { get; set; }
        public Company Company { get; set; }
    }
}