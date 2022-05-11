using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IBaBsReconciliationService
    {
        IResult Add(BaBsReconciliation babsReconciliation);
        IResult AddToExcel(string filePath, int companyId);
        IResult Update(BaBsReconciliation babsReconciliationn);
        IResult Delete(BaBsReconciliation babsReconciliation);
        IDataResult<BaBsReconciliation> GetById(int id);
        IDataResult<BaBsReconciliation> GetByCode(string code);
        IDataResult<List<BaBsReconciliation>> GetList(int companyId);
        IDataResult<List<BaBsReconciliation>> GetListByCurrencyAccountId(int currencyAccount);
        IDataResult<List<BaBsReconciliationDto>> GetListDto(int companyId);
        IResult SendReconciliationMail(BaBsReconciliationDto babsReconciliationDto);
    }
}