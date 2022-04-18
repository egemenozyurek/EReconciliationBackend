using Business.Abstract;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class AccountReconciliationDetailManager : IAccountReconciliationDetailService
    {
        private readonly IAccountReconciliationDetailDal _accountReconciliationDal;

        public AccountReconciliationDetailManager(IAccountReconciliationDetailDal accountReconciliationDal)
        {
            _accountReconciliationDal = accountReconciliationDal;
        }
    }
}