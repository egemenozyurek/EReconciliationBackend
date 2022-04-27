using Core.Entities;

namespace Entities.Concrete
{
    public class TermAndCondition : IEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}