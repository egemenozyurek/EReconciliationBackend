using Core.Entities;

namespace Entities.Concrete
{
    public class UserRelationship : IEntity
    {
        public int Id { get; set; }
        public int AdminUserId { get; set; }
        public int UserUserId { get; set; }
    }
}