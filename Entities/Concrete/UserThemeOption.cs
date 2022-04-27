using Core.Entities;

namespace Entities.Concrete
{
    public class UserThemeOption : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SidenavColor { get; set; }
        public string SidenavType { get; set; }
        public string Mode { get; set; }
    }
}