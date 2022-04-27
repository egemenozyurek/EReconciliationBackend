using Core.Entities.Concrete;

namespace Business.Abstract
{
    public interface IUserCompanyService
    {
        void Delete(UserCompany userCompany);
        UserCompany GetByUserIdAndCompanyId(int userId, int companyId);
        List<UserCompany> GetListByUserId(int userId);
    }
}