using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IForgotPasswordService
    {
        IDataResult<ForgotPassword> CreateForgotPassword(User user);
        IDataResult<List<ForgotPassword>> GetListByUserId(int userId);
        ForgotPassword GetForgotPassword(string value);
        void Update(ForgotPassword forgotPassword);
    }
}