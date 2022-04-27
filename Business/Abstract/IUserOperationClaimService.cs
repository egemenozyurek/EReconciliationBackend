using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IUserOperationClaimService
    {
        IResult Add(UserOperationClaim userOperationClaim);
        IResult Update(UserOperationClaim userOperationClaim);
        IResult Delete(UserOperationClaim userOperationClaim);
        IDataResult<UserOperationClaim> GetById(int id);
        IDataResult<List<UserOperationClaim>> GetList(int userId, int companyId);
        IDataResult<List<UserOperationClaimDto>> GetListDto(int userId, int companyId);
    }
}