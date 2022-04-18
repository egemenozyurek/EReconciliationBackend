using Business.Abstract;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class BaBsReconciliationManager : IBaBsReconciliationService
    {
        private readonly IBaBsReconciliationDal _baBsReconciliationDal;

        public BaBsReconciliationManager(IBaBsReconciliationDal baBsReconciliationDal)
        {
            _baBsReconciliationDal = baBsReconciliationDal;
        }
    }
}