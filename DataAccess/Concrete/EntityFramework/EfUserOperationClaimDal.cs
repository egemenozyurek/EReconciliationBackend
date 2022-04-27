using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Dtos;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserOperationClaimDal : EfEntityRepositoryBase<UserOperationClaim, ContextDb>, IUserOperationClaimDal
    {
        public List<UserOperationClaimDto> GetListDto(int userId, int companyId)
        {
            using (var context = new ContextDb())
            {
                var result = from userOperationClaim in context.UserOperationClaims.Where(x => x.UserId == userId && x.CompanyId == companyId)
                             join operationClaim in context.OperationClaims on userOperationClaim.OperationClaimId equals operationClaim.Id
                             select new UserOperationClaimDto
                             {
                                 UserId = userId,
                                 Id = operationClaim.Id,
                                 CompanyId = companyId,
                                 OperationClaimId =operationClaim.Id,
                                 OperationClaimDescription = operationClaim.Description,
                                 OperationClaimName =operationClaim.Name
                             };
                return result.ToList();
            }
        }
    }
}