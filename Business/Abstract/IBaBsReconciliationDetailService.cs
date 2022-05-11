using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IBaBsReconciliationDetailService
    {
        IResult Add(BaBsReconciliationDetail baBsReconciliationDetail);
        IResult AddToExcel(string filePath, int companyId);
        IResult Update(BaBsReconciliationDetail baBsReconciliationDetail);
        IResult Delete(BaBsReconciliationDetail baBsReconciliationDetail);
        IDataResult<BaBsReconciliationDetail> GetById(int id);
        IDataResult<List<BaBsReconciliationDetail>> GetList(int baBsReconciliationDetailId);
    }
}