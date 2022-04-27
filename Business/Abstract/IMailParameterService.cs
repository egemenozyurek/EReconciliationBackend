using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IMailParameterService
    {
        IResult Update(MailParameter mailParameter);
        IDataResult<MailParameter> Get(int companyId);
    }
}