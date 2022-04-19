using Core.Entities;

namespace Entities.Dtos
{
    public class UserForRegister : IDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}