using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class MailTemplateManager : IMailTemplateService
    {
        private readonly IMailTemplateDal _mailTemplateDal;

        public MailTemplateManager(IMailTemplateDal mailTemplateDal)
        {
            _mailTemplateDal = mailTemplateDal;
        }

        public IResult Add(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Add(mailTemplate);
            return new SuccessResult(Messages.MailTemplateAdded);
        }

        public IResult Delete(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Delete(mailTemplate);
            return new SuccessResult(Messages.MailTemplateDeleted);
        }

        public IDataResult<MailTemplate> Get(int id)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(m => m.Id == id));
        }

        public IDataResult<List<MailTemplate>> GetAll(int companyId)
        {
            return new SuccessDataResult<List<MailTemplate>>(_mailTemplateDal.GetList(m => m.CompanyId == companyId));
        }

        public IDataResult<MailTemplate> GetByCompanyId(int companyId)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(m => m.CompanyId == companyId));
        } 

        public IDataResult<MailTemplate> GetByTemplateName(string name, int companyId)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(m => m.Type == name && m.CompanyId == companyId));
        }

        public IResult Update(MailTemplate mailTemplate)
        {
            var result = _mailTemplateDal.Get(p => p.CompanyId == mailTemplate.CompanyId);
            if (result != null)
            {
                _mailTemplateDal.Update(mailTemplate);
            }
            else
            {
                mailTemplate.Id = 0;
                _mailTemplateDal.Add(mailTemplate);
            }            
            return new SuccessResult(Messages.MailTemplateUpdated);
        }
    }
}