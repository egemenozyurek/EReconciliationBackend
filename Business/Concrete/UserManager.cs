using Business.Abstract;
using Business.BusinessAspects;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
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
        private IUserRelationshipService _userRelationshipService;
        private IUserThemeOptionService _userThemeOptionService;

        public UserManager(IUserDal userDal, IOperationClaimService operationClaimService, IUserOperationClaimService userOperationClaimService, IUserCompanyService userCompanyService, ICompanyService companyService, IUserRelationshipService userRelationshipService, IUserThemeOptionService userThemeOptionService)
        {
            _userDal = userDal;
            _operationClaimService = operationClaimService;
            _userOperationClaimService = userOperationClaimService;
            _userCompanyService = userCompanyService;
            _companyService = companyService;
            _userRelationshipService = userRelationshipService;
            _userThemeOptionService = userThemeOptionService;
        }

        [CacheRemoveAspect("IUserService.Get")]
        [ValidationAspect(typeof(UserValidator))]
        public void Add(User user)
        {
            _userDal.Add(user);
        }

        public IDataResult<List<AdminCompaniesForUserDto>> GetAdminCompaniesForUser(int adminUserId, int userUserId)
        {
            return new SuccessDataResult<List<AdminCompaniesForUserDto>>(_userDal.GetAdminCompaniesForUser(adminUserId, userUserId));
        }

        [CacheAspect(60)]
        public User GetById(int id)
        {
            return _userDal.Get(u => u.Id == id);
        }

        [CacheAspect(60)]
        public IDataResult<User> GetByIdToResult(int id)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Id == id));
        }

        [CacheAspect(60)]
        public User GetByMail(string email)
        {
            return _userDal.Get(m => m.Email == email);
        }
        
        [CacheAspect(60)]
        public User GetByMailConfirmValue(string value)
        {
            return _userDal.Get(p => p.MailConfirmValue == value);
        }

        public List<OperationClaim> GetClaims(User user, int companyId)
        {
            return _userDal.GetClaims(user, companyId);
        }

        [SecuredOperation("User.Update,Admin")]
        public IDataResult<List<OperationClaimForUserListDto>> GetOperationClaimListForUser(string value, int companyId)
        {
            return new SuccessDataResult<List<OperationClaimForUserListDto>>(_userDal.GetOperationClaimListForUser(value, companyId));
        }

        public IDataResult<List<Company>> GetUserCompanyList(string value)
        {
            return new SuccessDataResult<List<Company>>(_userDal.GetUserCompanyList(value));
        }

        public IDataResult<List<UserCompanyDtoForList>> GetUserList(int companyId)
        {
            return new SuccessDataResult<List<UserCompanyDtoForList>>(_userDal.GetUserList(companyId));
        }

        public void Update(User user)
        {
            _userDal.Update(user);
        }

        public IResult UpdateOperationClaim(OperationClaimForUserListDto operationClaim)
        {
            if (operationClaim.Status is true)
            {
                var result = _userOperationClaimService.GetList(operationClaim.UserId, operationClaim.CompanyId).Data.Where(p => p.OperationClaimId == operationClaim.Id).FirstOrDefault();
                _userOperationClaimService.Delete(result);
            }
            else
            {
                UserOperationClaim userOperationClaim = new UserOperationClaim()
                {
                    CompanyId = operationClaim.CompanyId,
                    AddedAt = DateTime.Now,
                    IsActive = true,
                    OperationClaimId = operationClaim.Id,
                    UserId = operationClaim.UserId
                };

                _userOperationClaimService.Add(userOperationClaim);
            }
            return new SuccessResult(Messages.UpdatedUserOperationClaim);
        }

        public IResult UpdateResult(UserForRegisterToSecondAccountDto userForRegister)
        {
            var findUser = _userDal.Get(i => i.Id == userForRegister.Id);
            findUser.Name = userForRegister.Name;
            findUser.Email = userForRegister.Email;

            if (userForRegister.Password != "")
            {
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(userForRegister.Password, out passwordHash, out passwordSalt);
                findUser.PasswordHash = passwordHash;
                findUser.PasswordSalt = passwordSalt;
            }

            _userDal.Update(findUser);
            return new SuccessResult(Messages.UpdatedUser);
        }

        public IResult UserCompanyAdd(int userId, int companyId)
        {
            _companyService.UserCompanyAdd(userId, companyId);

            var operationClaims = _operationClaimService.GetList().Data;
            foreach (var operationClaim in operationClaims)
            {
                if (operationClaim.Name != "Admin" && operationClaim.Name != "MailParameter" && operationClaim.Name != "MailTemplete")
                {
                    UserOperationClaim userOperation = new UserOperationClaim()
                    {
                        CompanyId = companyId,
                        AddedAt = DateTime.Now,
                        IsActive = true,
                        OperationClaimId = operationClaim.Id,
                        UserId = userId
                    };
                    _userOperationClaimService.Add(userOperation);
                }
            }
            return new SuccessResult(Messages.AddedUserCompanyReletionShip);
        }

        public IResult UserCompanyDelete(int userId, int companyId)
        {
            var userOperationClaims = _userOperationClaimService.GetList(userId, companyId).Data;
            foreach (var userOperationCliam in userOperationClaims)
            {
                _userOperationClaimService.Delete(userOperationCliam);
            }

            var result = _userCompanyService.GetByUserIdAndCompanyId(userId, companyId);
            _userCompanyService.Delete(result);


            return new SuccessResult(Messages.DeletedUserCompanyReletionShip);
        }

        public IResult UserDelete(int userId)
        {
            var userCompanies = _userCompanyService.GetListByUserId(userId);
            foreach (var company in userCompanies)
            {
                var userOperationClaims = _userOperationClaimService.GetList(userId, company.CompanyId).Data;
                foreach (var userOperationCliam in userOperationClaims)
                {
                    _userOperationClaimService.Delete(userOperationCliam);
                }

                var result = _userCompanyService.GetByUserIdAndCompanyId(userId, company.CompanyId);
                _userCompanyService.Delete(result);
            }

            var userReletionShips = _userRelationshipService.GetList(userId);
            foreach (var userReletionShip in userReletionShips)
            {
                _userRelationshipService.Delete(userReletionShip);
            }

            var theme = _userThemeOptionService.GetByUserId(userId).Data;
            _userThemeOptionService.Delete(theme);

            var user = _userDal.Get(p => p.Id == userId);
            _userDal.Delete(user);
            return new SuccessResult(Messages.DeletedUser);
        }
    }
}