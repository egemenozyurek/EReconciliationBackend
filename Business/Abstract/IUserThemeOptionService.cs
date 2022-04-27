using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IUserThemeOptionService
    {
        IResult Update(UserThemeOption userThemeOption);
        void Delete (UserThemeOption userThemeOption);
        IDataResult<UserThemeOption> GetByUserId(int userId);
    }
}