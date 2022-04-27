using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Dtos;

namespace Business.Concrete
{
    public class MailManager : IMailService
    {
        private readonly IMailDal _mailDal;

        public MailManager(IMailDal mailDal)
        {
            _mailDal = mailDal;
        }

        public IResult SendMail(SendMailDto sendMailDto)
        {
            _mailDal.SendMail(sendMailDto);
            return new SuccessResult(Messages.MailSendSucessful);
        }
    }
}