using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using ExcelDataReader;

namespace Business.Concrete
{
    public class BaBsReconciliationManager : IBaBsReconciliationService
    {
        private readonly IBaBsReconciliationDal _baBsReconciliationDal;
        private readonly ICurrencyAccountService _currencyAccountService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IMailParameterService _mailParameterService;

        public BaBsReconciliationManager(IBaBsReconciliationDal baBsReconciliationDal, ICurrencyAccountService currencyAccountService, IMailService mailService, IMailTemplateService mailTemplateService, IMailParameterService mailParameterService)
        {
            _baBsReconciliationDal = baBsReconciliationDal;
            _currencyAccountService = currencyAccountService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
            _mailParameterService = mailParameterService;
        }

        public IResult Add(BaBsReconciliation babsReconciliation)
        {
            string guid = Guid.NewGuid().ToString();
            babsReconciliation.Guid = guid;
            _baBsReconciliationDal.Add(babsReconciliation);
            return new SuccessResult(Messages.AddedBaBsReconciliation);
        }

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
                            string type = reader.GetString(1);
                            double mounth = reader.GetDouble(2);
                            double year = reader.GetDouble(3);
                            double quantit = reader.GetDouble(4);
                            double total = reader.GetDouble(5);
                            string guid = Guid.NewGuid().ToString();

                            int currencyAccountId = _currencyAccountService.GetByCode(code, companyId).Data.Id;

                            BaBsReconciliation baBsReconciliation = new BaBsReconciliation()
                            {
                                CompanyId = companyId,
                                CurrencyAccountId = currencyAccountId,
                                Type = type,
                                Mounth = Convert.ToInt16(mounth),
                                Year = Convert.ToInt16(year),
                                Quantity = Convert.ToInt16(quantit),
                                Total = Convert.ToDecimal(total),
                                Guid = guid
                            };

                            _baBsReconciliationDal.Add(baBsReconciliation);
                        }
                    }
                }
            }

            File.Delete(filePath);

            return new SuccessResult(Messages.AddedBaBsReconciliation);
        }

        public IResult Delete(BaBsReconciliation babsReconciliation)
        {
            _baBsReconciliationDal.Delete(babsReconciliation);
            return new SuccessResult(Messages.DeletedBaBsReconciliation);
        }

        public IDataResult<BaBsReconciliation> GetByCode(string code)
        {
            return new SuccessDataResult<BaBsReconciliation>(_baBsReconciliationDal.Get(p => p.Guid == code));
        }

        public IDataResult<BaBsReconciliation> GetById(int id)
        {
            return new SuccessDataResult<BaBsReconciliation>(_baBsReconciliationDal.Get(p => p.Id == id));
        }

        public IDataResult<List<BaBsReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<BaBsReconciliation>>(_baBsReconciliationDal.GetList(p => p.CompanyId == companyId));
        }

        public IDataResult<List<BaBsReconciliation>> GetListByCurrencyAccountId(int currencyAccount)
        {
            return new SuccessDataResult<List<BaBsReconciliation>>(_baBsReconciliationDal.GetList(p => p.CurrencyAccountId == currencyAccount));
        }

        public IDataResult<List<BaBsReconciliationDto>> GetListDto(int companyId)
        {
            return new SuccessDataResult<List<BaBsReconciliationDto>>(_baBsReconciliationDal.GetAllDto(companyId));
        }

        public IResult SendReconciliationMail(BaBsReconciliationDto babsReconciliationDto)
        {
            string subject = "Mutabakat Maili";
            string body = $"Şirket Adımız: {babsReconciliationDto.CompanyName} <br /> " +
                $"Şirket Vergi Dairesi: {babsReconciliationDto.CompanyTaxDepartment} <br />" +
                $"Şirket Vergi Numarası: {babsReconciliationDto.CompanyTaxIdNumber} - {babsReconciliationDto.CompanyIdentityNumber} <br /><hr>" +
                $"Sizin Şirket: {babsReconciliationDto.AccountName} <br />" +
                $"Sizin Şirket Vergi Dairesi: {babsReconciliationDto.AccountTaxDepartment} <br />" +
                $"Sizin Şirket Vergi Numarası: {babsReconciliationDto.AccountTaxIdNumber} - {babsReconciliationDto.AccountIdentityNumber} <br /><hr>" +
                $"Ay / Yıl: {babsReconciliationDto.Mounth} / {babsReconciliationDto.Year} <br />" +
                $"Adet: {babsReconciliationDto.Quantity} <br />" +
                $"Tutar: {babsReconciliationDto.Total} {babsReconciliationDto.CurrencyCode} <br />";
            string link = "https://localhost:7220/api/BaBsReconciliations/GetByCode?code=" + babsReconciliationDto.Guid;
            string linkDescription = "Mutabakatı Cevaplamak için Tıklayın";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Kayıt", 4);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);


            var mailParameter = _mailParameterService.Get(4);
            Entities.Dtos.SendMailDto sendMailDto = new Entities.Dtos.SendMailDto()
            {
                MailParameter = mailParameter.Data,
                Email = babsReconciliationDto.AccountEmail,
                Subject = subject,
                Body = templateBody
            };

            _mailService.SendMail(sendMailDto);

            return new SuccessResult(Messages.MailSendSucessful);
        }

        public IResult Update(BaBsReconciliation babsReconciliationn)
        {
            _baBsReconciliationDal.Update(babsReconciliationn);
            return new SuccessResult(Messages.UpdatedBaBsReconciliation);
        }
    }
}