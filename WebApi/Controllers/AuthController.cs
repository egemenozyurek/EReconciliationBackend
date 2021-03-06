using Business.Abstract;
using Business.Constants;
using Core.Utilities.Hashing;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IForgotPasswordService _forgotPasswordService;

        public AuthController(IAuthService authService, IForgotPasswordService forgotPasswordService)
        {
            _authService = authService;
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpPost("registersecondaccount")]
        public IActionResult RegisterSecondAccount(UserForRegisterToSecondAccountDto userForRegister)
        {
            var userExists = _authService.UserExists(userForRegister.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var registerResult = _authService.RegisterSecondAccount(userForRegister, userForRegister.Password, userForRegister.CompanyId, userForRegister.AdminUserId);
            if (registerResult.Success)
            {
                return Ok(registerResult);
            }

            return BadRequest(registerResult.Message);
        }

        [HttpPost("register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegister)
        {
            var userExist = _authService.UserExists(userAndCompanyRegister.UserForRegister.Email);
            if (!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }

            var companyExist = _authService.CompanyExists(userAndCompanyRegister.Company);
            if (!companyExist.Success)
            {
                return BadRequest(userExist.Message);
            }

            var registerResult = _authService.Register(userAndCompanyRegister.UserForRegister, userAndCompanyRegister.UserForRegister.Password, userAndCompanyRegister.Company);

            var result = _authService.CreateAccessToken(registerResult.Data, registerResult.Data.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            // if (registerResult.Success)
            // {
            //     return Ok(registerResult);
            // }

            return BadRequest(registerResult.Message);
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var userToLogin = _authService.Login(userForLogin);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            if (userToLogin.Data.IsActive)
            {
                if (userToLogin.Data.MailConfirm)
                {
                    var userCompany = _authService.GetCompany(userToLogin.Data.Id).Data;
                    var result = _authService.CreateAccessToken(userToLogin.Data, userCompany.CompanyId);
                    if (result.Success)
                    {
                        return Ok(result);
                    }
                    return BadRequest(result);
                }
                return BadRequest("Gelen onay mailini cevaplamal??s??n??z. Mail adresinizi onaylamadan sisteme giri?? yapamazs??n??z!");
            }
            return BadRequest("Kullan??c?? pasif durumda. Aktif etmek i??in y??neticinize dan??????n.");
        }

        [HttpGet("confirmuser")]
        public IActionResult ConfirmUser(string value)
        {
            var user = _authService.GetByMailConfirmValue(value).Data;
            if (user.MailConfirm)
            {
                return BadRequest("Kullan??c?? maili zaten onayl??. Ayn?? maili tekrar onaylayamazs??n??z");
            }

            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
            var result = _authService.Update(user);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpGet("sendconfirmemail")]
        public IActionResult SendConfirmEmail(string email)
        {
            var user = _authService.GetByEmail(email).Data;

            if (user == null)
            {
                return BadRequest("Kullan??c?? Bulunamad??");
            }

            if (user.MailConfirm)
            {
                return BadRequest("Kullan??c?? maili onayl??");
            }

            var result = _authService.SendConfirmEmailAgain(user);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpGet("forgotpassword")]
        public IActionResult ForgotPassword(string email)
        {
            var user = _authService.GetByEmail(email).Data;

            if (user is null)
            {
                return BadRequest("Kullan??c?? Bulunamad??");
            }

            var lists = _forgotPasswordService.GetListByUserId(user.Id).Data;
            foreach (var item in lists)
            {
                item.IsActive = false;
                _forgotPasswordService.Update(item);
            }

            var forgotPassword = _forgotPasswordService.CreateForgotPassword(user).Data;

            var result = _authService.SendForgotPasswordEmail(user, forgotPassword.Value);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("forgotPasswordLinkCheck")]
        public IActionResult ForgotPasswordLinkCheck(string value)
        {
            var result = _forgotPasswordService.GetForgotPassword(value);
            if (result is null)
            {
                return BadRequest("T??klad??????n??z link ge??ersiz");
            }

            if (result.IsActive == true)
            {
                DateTime date1 = DateTime.Now.AddHours(-1);
                DateTime date2 = DateTime.Now;
                if (result.SendDate >= date1 && result.SendDate <= date2)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest("T??klad??????n??z link ge??ersiz");
                }
            }
            else
            {
                return BadRequest("T??klad??????n??z link ge??ersiz");
            }
        }

        [HttpPost("changePasswordToForgotPassword")]
        public IActionResult ChangePasswordToForgotPassword(ForgotPasswordDto passwordDto)
        {
            var forgotPasswordResult = _forgotPasswordService.GetForgotPassword(passwordDto.Value);
            forgotPasswordResult.IsActive = false;
            _forgotPasswordService.Update(forgotPasswordResult);

            var userResult = _authService.GetById(forgotPasswordResult.UserId).Data;
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(passwordDto.Password, out passwordHash, out passwordSalt);
            userResult.PasswordHash = passwordHash;
            userResult.PasswordSalt = passwordSalt;

            var result = _authService.ChangePassword(userResult);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
    }
}