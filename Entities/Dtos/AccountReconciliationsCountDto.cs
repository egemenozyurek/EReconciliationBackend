using Core.Entities;

namespace Entities.Dtos
{
    public class AccountReconciliationsCountDto : IDto
    {
        public int AllReconciliation { get; set; }
        public int SucceedReconciliation { get; set; }
        public int NotResponseReconciliation { get; set; }
        public int FailReconciliation { get; set; }
    }
}