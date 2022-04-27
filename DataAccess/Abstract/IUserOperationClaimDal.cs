using Core.DataAccess;
using Core.Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IUserOperationClaimDal : IEntityRepository<UserOperationClaim>
    {
        List<UserOperationClaimDto> GetListDto(int userId, int companyId);
    }
}