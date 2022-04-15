using Core.Entities;

namespace Entities.Concrete
{
    public class BaBsReconciliationDetail : IEntity{
        public int Id { get; set; }
        public int BaBsReconciliationId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}