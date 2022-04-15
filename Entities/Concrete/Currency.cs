using Core.Entities;

namespace Entities.Concrete
{
    public class Currency : IEntity{
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}