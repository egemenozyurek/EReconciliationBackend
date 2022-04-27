using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IMailDal
    {
        void SendMail(SendMailDto sendMailDto);
    }
}