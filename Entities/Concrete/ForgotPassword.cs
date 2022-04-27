using Core.Entities;

namespace Entities.Concrete
{
    public class ForgotPassword : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Value { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsActive { get; set; }
    }
}