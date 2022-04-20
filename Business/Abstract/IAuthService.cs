using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> Register(UserForRegister userForRegister, string password);
        IDataResult<User> Login(UserForLogin userForLogin);
        IResult UserExist(string email);
        IDataResult<AccessToken> CreateAccessToken(User user,int companyId);
    }
}