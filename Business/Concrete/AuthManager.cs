using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transactions;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;
        private readonly IMailService _mailService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IOperationClaimService _operationClaimService;
        private readonly IUserOperationClaimService _userOperationClaimService;
        private readonly IUserRelationshipService _userRelationshipService;
        private readonly IUserThemeOptionService _userThemeOptionService;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService, IMailService mailService, IMailParameterService mailParameterService, IMailTemplateService mailTemplateService, IOperationClaimService operationClaimService, IUserOperationClaimService userOperationClaimService, IUserRelationshipService userRelationshipService, IUserThemeOptionService userThemeOptionService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
            _mailService = mailService;
            _mailParameterService = mailParameterService;
            _mailTemplateService = mailTemplateService;
            _operationClaimService = operationClaimService;
            _userOperationClaimService = userOperationClaimService;
            _userRelationshipService = userRelationshipService;
            _userThemeOptionService = userThemeOptionService;
        }

        public IResult ChangePassword(User user)
        {
            _userService.Update(user);
            return new SuccessResult(Messages.ChangedPassword);
        }

        public IResult CompanyExists(Company company)
        {
            var result = _companyService.CompanyExists(company);
            if (!result.Success)
            {
                return new ErrorResult(Messages.CompanyAlreadyExists);
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user, int companyId)
        {
            var claims = _userService.GetClaims(user, companyId);
            var company = _companyService.GetById(companyId).Data;
            var accessToken = _tokenHelper.CreateToken(user, claims, companyId, company.Name);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
        }

        public IDataResult<User> GetByEmail(string email)
        {
            return new SuccessDataResult<User>(_userService.GetByMail(email));
        }

        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userService.GetById(id));
        }

        public IDataResult<User> GetByMailConfirmValue(string value)
        {
            return new SuccessDataResult<User>(_userService.GetByMailConfirmValue(value));
        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyService.GetCompany(userId).Data);
        }

        public IDataResult<User> Login(UserForLogin userForLogin)
        {
            var userToCheck = _userService.GetByMail(userForLogin.Email);
            if (userToCheck is null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }

            if (!HashingHelper.VerifyPasswordHash(userForLogin.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }

            return new SuccessDataResult<User>(userToCheck, Messages.SuccessfulLogin);
        }

        [TransactionScopeAspect]
        public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password, Company company)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name
            };

            ValidationTool.Validate(new UserValidator(), user);
            ValidationTool.Validate(new CompanyValidator(), company);

            _userService.Add(user);
            _companyService.Add(company);

            _companyService.UserCompanyAdd(user.Id, company.Id);

            UserCompanyDto userCompanyDto = new UserCompanyDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AddedAt = user.AddedAt,
                CompanyId = company.Id,
                IsActive = true,
                MailConfirm = user.MailConfirm,
                MailConfirmDate = user.MailConfirmDate,
                MailConfirmValue = user.MailConfirmValue,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt
            };

            SendConfirmEmail(user);

            var operationClaims = _operationClaimService.GetList().Data;
            foreach (var operationClaim in operationClaims)
            {
                if (operationClaim.Name != "Admin")
                {
                    UserOperationClaim userOperation = new UserOperationClaim()
                    {
                        CompanyId = company.Id,
                        AddedAt = DateTime.Now,
                        IsActive = true,
                        OperationClaimId = operationClaim.Id,
                        UserId = user.Id
                    };
                    _userOperationClaimService.Add(userOperation);
                }
            }

            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Messages.UserRegistered);
        }

        void SendConfirmEmail(User user)
        {
            string subject = "Kullan??c?? Kay??t Onay Maili";
            string body = "Kullan??c??n??z sisteme kay??t oldu. Kayd??n??z?? tamamlamak i??in a??a????daki linke t??klaman??z gerekmektedir.";
            string link = "http://localhost:4200/registerConfirm/" + user.MailConfirmValue;
            string linkDescription = "Kayd?? Onaylamak i??in T??klay??n";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Kay??t", 4);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);


            var mailParameter = _mailParameterService.Get(4);
            SendMailDto sendMailDto = new SendMailDto()
            {
                MailParameter = mailParameter.Data,
                Email = user.Email,
                Subject = "Kullan??c?? Kay??t Onay Maili",
                Body = templateBody
            };

            _mailService.SendMail(sendMailDto);

            user.MailConfirmDate = DateTime.Now;
            _userService.Update(user);
        }

        [TransactionScopeAspect]
        public IDataResult<List<UserRelationshipDto>> RegisterSecondAccount(UserForRegister userForRegister, string password, int companyId, int adminUserId)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name
            };

            _userService.Add(user);

            _companyService.UserCompanyAdd(user.Id, companyId);

            var operationClaims = _operationClaimService.GetList().Data;
            foreach (var operationClaim in operationClaims)
            {
                if (operationClaim.Name != "Admin")
                {
                    UserOperationClaim userOperation = new UserOperationClaim()
                    {
                        CompanyId = companyId,
                        AddedAt = DateTime.Now,
                        IsActive = true,
                        OperationClaimId = operationClaim.Id,
                        UserId = user.Id
                    };
                    _userOperationClaimService.Add(userOperation);
                }
            }

            UserRelationship userRelationship = new UserRelationship
            {
                UserUserId = user.Id,
                AdminUserId = adminUserId
            };

            _userRelationshipService.Add(userRelationship);

            var result = _userRelationshipService.GetListDto(adminUserId).Data;

            UserThemeOption userThemeOption = new UserThemeOption()
            {
                UserId = user.Id,
                SidenavColor = "primary",
                SidenavType = "dark",
                Mode = ""
            };

            _userThemeOptionService.Update(userThemeOption);

            SendConfirmEmail(user);

            return new SuccessDataResult<List<UserRelationshipDto>>(result, Messages.UserRegistered);
        }

        public IResult SendConfirmEmailAgain(User user)
        {
            if (user.MailConfirm is true)
            {
                return new ErrorResult(Messages.MailAlreadyConfirm);
            }

            DateTime confirmMailDate = user.MailConfirmDate;
            DateTime now = DateTime.Now;
            if (confirmMailDate.ToShortDateString() == now.ToShortDateString())
            {
                if (confirmMailDate.Hour == now.Hour && confirmMailDate.AddMinutes(5).Minute <= now.Minute)
                {
                    SendConfirmEmail(user);
                    return new SuccessResult(Messages.MailConfirmSendSuccessful);
                }
                else
                {
                    return new ErrorResult(Messages.MailConfirmTimeHasNotExpired);
                }
            }
            SendConfirmEmail(user);
            return new SuccessResult(Messages.MailConfirmSendSuccessful);
        }

        public IResult SendForgotPasswordEmail(User user, string value)
        {
            string subject = "??ifremi Unuttum";
            string body = "e-Mutabakat sitemizden ??ifrenizi unuttu??unuzu belirttiniz. A??a????daki linkte t??klayarak ??ifrenizi yeniden belirleyebilirsiniz. Linkin 1 saat s??resi vard??r. S??re sonunda kullan??lamaz. ??yi g??nler dileriz.";
            string link = "http://localhost:4200/forgot-password/" + value;
            string linkDescription = "??ifrenizi Tekrar Belirlemek i??in T??klay??n";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Kay??t", 4);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);


            var mailParameter = _mailParameterService.Get(4);
            SendMailDto sendMailDto = new SendMailDto()
            {
                MailParameter = mailParameter.Data,
                Email = user.Email,
                Subject = subject,
                Body = templateBody
            };

            _mailService.SendMail(sendMailDto);
            return new SuccessResult(Messages.MailSendSucessful);
        }

        public IResult Update(User user)
        {
            _userService.Update(user);
            return new SuccessResult(Messages.UpdatedUser);
        }

        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }
    }
}