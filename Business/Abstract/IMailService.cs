using Core.Utilities.Results.Abstract;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IMailService
    {
        IResult SendMail(SendMailDto sendMailDto);
    }
}