using Business.Abstract;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class MailParameterManager : IMailParameterService
    {
        private readonly IMailParameterDal _mailParameterDal;

        public MailParameterManager(IMailParameterDal mailParameterDal)
        {
            _mailParameterDal = mailParameterDal;
        }
    }
}