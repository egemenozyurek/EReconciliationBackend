using Business.Abstract;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class CurrencyAccountManager : ICurrencyAccountService
    {
        private readonly ICurrencyAccountDal _currenctAccountDal;

        public CurrencyAccountManager(ICurrencyAccountDal currenctAccountDal)
        {
            _currenctAccountDal = currenctAccountDal;
        }
    }
}