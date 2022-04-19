using Core.Entities;

namespace Entities.Dtos
{
    public class UserForLogin : IDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}