using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;
        private IOperationClaimService _operationClaimService;
        private IUserOperationClaimService _userOperationClaimService;
        private IUserCompanyService _userCompanyService;
        private ICompanyService _companyService;

        public UserManager(IUserDal userDal, IOperationClaimService operationClaimService, IUserOperationClaimService userOperationClaimService, IUserCompanyService userCompanyService, ICompanyService companyService)
        {
            _userDal = userDal;
            _operationClaimService = operationClaimService;
            _userOperationClaimService = userOperationClaimService;
            _userCompanyService = userCompanyService;
            _companyService = companyService;
        }

        public void Add(User user)
        {
            _userDal.Add(user);
        }

        public IDataResult<List<AdminCompaniesForUserDto>> GetAdminCompaniesForUser(int adminUserId, int userUserId)
        {
            return new SuccessDataResult<List<AdminCompaniesForUserDto>>(_userDal.GetAdminCompaniesForUser(adminUserId, userUserId));
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IDataResult<User> GetByIdToResult(int id)
        {
            throw new NotImplementedException();
        }

        public User GetByMail(string email)
        {
            throw new NotImplementedException();
        }

        public User GetByMailConfirmValue(string value)
        {
            throw new NotImplementedException();
        }

        public List<OperationClaim> GetClaims(User user, int companyId)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<OperationClaimForUserListDto>> GetOperationClaimListForUser(string value, int companyId)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<Company>> GetUserCompanyList(string value)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<UserCompanyDtoForList>> GetUserList(int companyId)
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }

        public IResult UpdateOperationClaim(OperationClaimForUserListDto operationClaim)
        {
            throw new NotImplementedException();
        }

        public IResult UpdateResult(UserForRegisterToSecondAccountDto userForRegister)
        {
            throw new NotImplementedException();
        }

        public IResult UserCompanyAdd(int userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public IResult UserCompanyDelete(int userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public IResult UserDelete(int userId)
        {
            throw new NotImplementedException();
        }
    }
}