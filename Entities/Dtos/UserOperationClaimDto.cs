using Core.Entities;

namespace Entities.Dtos
{
    public class UserOperationClaimDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }
        public string OperationClaimName { get; set; }
        public string OperationClaimDescription { get; set; }
        public int CompanyId { get; set; }   
    }
}