using Core.DataAccess;
using Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IAccountReconciliationDal : IEntityRepository<AccountReconciliation>
    {
        List<AccountReconciliationDto> GetAllDto(int companyId);
        AccountReconciliationDto GetByIdDto(int id);
        AccountReconciliationDto GetByCodeDto(string code);
        AccountReconciliationsCountDto GetCountDto(int companyId);
    }
}