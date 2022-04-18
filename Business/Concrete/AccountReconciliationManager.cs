using Business.Abstract;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class AccountReconciliationManager : IAccountReconciliationService
    {
        private readonly IAccountReconciliationDal _accountReconciliationDal;

        public AccountReconciliationManager(IAccountReconciliationDal accountReconciliationDal)
        {
            _accountReconciliationDal = accountReconciliationDal;
        }
    }
}