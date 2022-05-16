using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class CurrencyManager : ICurrencyService
    {
        private readonly ICurrencyDal _currencyDal;

        public CurrencyManager(ICurrencyDal currencyDal)
        {
            _currencyDal = currencyDal;
        }

        public IDataResult<List<Currency>> GetList()
        {
            return new SuccessDataResult<List<Currency>>(_currencyDal.GetList());
        }
    }   
}