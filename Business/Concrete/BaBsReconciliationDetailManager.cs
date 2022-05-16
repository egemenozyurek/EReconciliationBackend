using Business.Abstract;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspect.Performance;
using Core.Aspects.Autofac.Transactions;
using Core.Aspects.Caching;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using ExcelDataReader;

namespace Business.Concrete
{
    public class BaBsReconciliationDetailManager : IBaBsReconciliationDetailService
    {
        private readonly IBaBsReconciliationDetailDal _baBsReconciliationDetailDal;

        public BaBsReconciliationDetailManager(IBaBsReconciliationDetailDal baBsReconciliationDal)
        {
            _baBsReconciliationDetailDal = baBsReconciliationDal;
        }

        [PerformanceAspect(3)]
        [SecuredOperation("BaBsReconciliationDetail.Add,Admin")]
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        public IResult Add(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            _baBsReconciliationDetailDal.Add(baBsReconciliationDetail);
            return new SuccessResult(Messages.AddedBaBsReconciliationDetail);
        }

        [PerformanceAspect(3)]
        [SecuredOperation("BaBsReconciliationDetail.Add,Admin")]
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int baBsReconciliationId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string description = reader.GetString(1);

                        if (description != "Açıklama" && description != null)
                        {
                            DateTime date = reader.GetDateTime(0);
                            double amount = reader.GetDouble(2);

                            BaBsReconciliationDetail baBsReconciliationDetail = new BaBsReconciliationDetail()
                            {
                                BaBsReconciliationId = baBsReconciliationId,
                                Date = date,
                                Description = description,
                                Amount = Convert.ToDecimal(amount)
                            };

                            _baBsReconciliationDetailDal.Add(baBsReconciliationDetail);
                        }
                    }
                }
            }

            File.Delete(filePath);
            return new SuccessResult(Messages.AddedAccountReconciliation);
        }

        [PerformanceAspect(3)]
        [SecuredOperation("BaBsReconciliationDetail.Delete,Admin")]
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        public IResult Delete(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            _baBsReconciliationDetailDal.Delete(baBsReconciliationDetail);
            return new SuccessResult(Messages.DeletedBaBsReconciliationDetail);
        }

        [PerformanceAspect(3)]
        [SecuredOperation("BaBsReconciliationDetail.Get,Admin")]
        [CacheAspect(60)]
        public IDataResult<BaBsReconciliationDetail> GetById(int id)
        {
            return new SuccessDataResult<BaBsReconciliationDetail>(_baBsReconciliationDetailDal.Get(p => p.Id == id));
        }

        [CacheAspect(60)]
        public IDataResult<List<BaBsReconciliationDetail>> GetList(int baBsReconciliationDetailId)
        {
            return new SuccessDataResult<List<BaBsReconciliationDetail>>(_baBsReconciliationDetailDal.GetList(p => p.BaBsReconciliationId == baBsReconciliationDetailId));
        }

        [PerformanceAspect(3)]
        [SecuredOperation("BaBsReconciliationDetail.Update,Admin")]
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        public IResult Update(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            _baBsReconciliationDetailDal.Update(baBsReconciliationDetail);
            return new SuccessResult(Messages.UpdatedBaBsReconciliationDetail);
        }
    }
}