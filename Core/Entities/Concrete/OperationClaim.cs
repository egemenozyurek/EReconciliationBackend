using Core.Entities;

namespace Core.Entities.Concrete
{
    public class OperationClaim : IEntity {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedAt { get; set; }
        public bool IsActive { get; set; }
    }
}