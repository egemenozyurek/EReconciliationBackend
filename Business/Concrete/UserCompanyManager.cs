using Business.Abstract;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class UserCompanyManager : IUserCompanyService
    {
        private readonly IUserCompanyDal _userCompanyDal;

        public UserCompanyManager(IUserCompanyDal userCompanyDal)
        {
            _userCompanyDal = userCompanyDal;
        }

        public void Delete(UserCompany userCompany)
        {
            _userCompanyDal.Delete(userCompany);
        }

        public UserCompany GetByUserIdAndCompanyId(int userId, int companyId)
        {
            return _userCompanyDal.Get(p => p.UserId == userId && p.CompanyId == companyId);
        }

        public List<UserCompany> GetListByUserId(int userId)
        {
            return _userCompanyDal.GetList(p => p.UserId == userId);
        }
    }
}