using Business.Abstract;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Transactions;
using Core.Aspects.Caching;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using ExcelDataReader;

namespace Business.Concrete
{
    public class AccountReconciliationManager : IAccountReconciliationService
    {
        private readonly IAccountReconciliationDal _accountReconciliationDal;
        private readonly IAccountReconciliationDetailService _accountReconciliationDetailService;
        private readonly ICurrencyAccountService _currencyAccountService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IMailParameterService _mailParameterService;

        public AccountReconciliationManager(IAccountReconciliationDal accountReconciliationDal, ICurrencyAccountService currencyAccountService, IMailService mailService, IMailTemplateService mailTemplateService, IMailParameterService mailParameterService, IAccountReconciliationDetailService accountReconciliationDetailService)
        {
            _accountReconciliationDal = accountReconciliationDal;
            _currencyAccountService = currencyAccountService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
            _mailParameterService = mailParameterService;
            _accountReconciliationDetailService = accountReconciliationDetailService;
        }

        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        [SecuredOperation("AccountReconciliation.Add")]
        public IResult Add(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Add(accountReconciliation);
            return new SuccessResult(Messages.AddedAccountReconciliation);
        }

        [TransactionScopeAspect]
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult AddToExcel(string filePath, int companyId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString(0);

                        if (code != "Cari Kodu" && code != null)
                        {
                            DateTime startingDate = reader.GetDateTime(1);
                            DateTime endingDate = reader.GetDateTime(2);
                            double currencyId = reader.GetDouble(3);
                            double debit = reader.GetDouble(4);
                            double credit = reader.GetDouble(5);

                            int currencyAccountId = _currencyAccountService.GetByCode(code, companyId).Data.Id;
                            string guid = Guid.NewGuid().ToString();

                            AccountReconciliation accountReconciliation = new AccountReconciliation()
                            {
                                CompanyId = companyId,
                                CurrencyAccountId = currencyAccountId,
                                CurrencyCredit = Convert.ToDecimal(credit),
                                CurrencyDebit = Convert.ToDecimal(debit),
                                CurrencyId = Convert.ToInt16(currencyId),
                                StartingDate = startingDate,
                                EndingDate = endingDate,
                                Guid = guid
                            };

                            _accountReconciliationDal.Add(accountReconciliation);
                        }
                    }
                }
            }

            File.Delete(filePath);

            return new SuccessResult(Messages.AddedAccountReconciliation);
        }

        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        [TransactionScopeAspect]
        public IResult Delete(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Delete(accountReconciliation);
            return new SuccessResult(Messages.DeletedAccountReconciliation);
        }

        public IDataResult<AccountReconciliation> GetByCode(string code)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliationDal.Get(p => p.Guid == code));
        }

        public IDataResult<AccountReconciliationDto> GetByCodeDto(string code)
        {
            return new SuccessDataResult<AccountReconciliationDto>(_accountReconciliationDal.GetByCodeDto(code));
        }

        public IDataResult<AccountReconciliation> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliationDal.Get(p => p.Id == id));
        }

        public IDataResult<AccountReconciliationsCountDto> GetCountDto(int companyId)
        {
            return new SuccessDataResult<AccountReconciliationsCountDto>(_accountReconciliationDal.GetCountDto(companyId));
        }

        public IDataResult<List<AccountReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliation>>(_accountReconciliationDal.GetList(p => p.CompanyId == companyId));
        }

        public IDataResult<List<AccountReconciliation>> GetListByCurrencyAccountId(int currencyAccountId)
        {
            return new SuccessDataResult<List<AccountReconciliation>>(_accountReconciliationDal.GetList(p => p.CurrencyAccountId == currencyAccountId));
        }

        public IDataResult<List<AccountReconciliationDto>> GetListDto(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliationDto>>(_accountReconciliationDal.GetAllDto(companyId));
        }

        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Result(ReconciliationResultDto reconciliationResultDto)
        {
            var result = _accountReconciliationDal.Get(p => p.Id == reconciliationResultDto.Id);
            result.IsResultSuccceed = reconciliationResultDto.Result;
            result.ResultDate = DateTime.Now;
            result.ResultNote = "Cevaplayan: " + reconciliationResultDto.Name + " Not: " + reconciliationResultDto.Note;
            _accountReconciliationDal.Update(result);
            return new SuccessResult(Messages.ReconciliationResultSucceed);
        }

        public IResult SendReconciliationMail(int id)
        {
            var accountReconciliationDto = _accountReconciliationDal.GetByIdDto(id);

            if (accountReconciliationDto.IsResultSuccceed == true)
            {
                return new ErrorResult(Messages.IsReconciliationAlreadySucceed);
            }
            string subject = "Mutabakat Maili";
            string body = $"Şirket Adımız: {accountReconciliationDto.CompanyName} <br /> " +
                $"Şirket Vergi Dairesi: {accountReconciliationDto.CompanyTaxDepartment} <br />" +
                $"Şirket Vergi Numarası: {accountReconciliationDto.CompanyTaxIdNumber} - {accountReconciliationDto.CompanyIdentityNumber} <br /><hr>" +
                $"Sizin Şirket: {accountReconciliationDto.AccountName} <br />" +
                $"Sizin Şirket Vergi Dairesi: {accountReconciliationDto.AccountTaxDepartment} <br />" +
                $"Sizin Şirket Vergi Numarası: {accountReconciliationDto.AccountTaxIdNumber} - {accountReconciliationDto.AccountIdentityNumber} <br /><hr>" +
                $"Borç: {accountReconciliationDto.CurrencyDebit} {accountReconciliationDto.CurrencyCode} <br />" +
                $"Alacak: {accountReconciliationDto.CurrencyCredit} {accountReconciliationDto.CurrencyCode} <br />";
            string link = "http://localhost:4200/account-reconciliation-result/" + accountReconciliationDto.Guid;
            string linkDescription = "Mutabakatı Cevaplamak için Tıklayın";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Mutabakat", accountReconciliationDto.CompanyId);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);


            var mailParameter = _mailParameterService.Get(4);
            Entities.Dtos.SendMailDto sendMailDto = new Entities.Dtos.SendMailDto()
            {
                MailParameter = mailParameter.Data,
                Email = accountReconciliationDto.AccountEmail,
                Subject = subject,
                Body = templateBody
            };

            _mailService.SendMail(sendMailDto);

            return new SuccessResult(Messages.MailSendSucessful);
        }

        public IResult Update(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Update(accountReconciliation);
            return new SuccessResult(Messages.UpdatedAccountReconciliation);
        }

        public IResult UpdateResult(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Update(accountReconciliation);
            return new SuccessResult(Messages.UpdatedAccountReconciliation);
        }
    }
}