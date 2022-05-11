using Core.DataAccess;
using Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IBaBsReconciliationDal : IEntityRepository<BaBsReconciliation>
    {
        List<BaBsReconciliationDto> GetAllDto(int companyId);
    }
}