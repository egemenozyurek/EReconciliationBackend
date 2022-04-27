using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface ITermAndConditionService
    {
        IResult Update(TermAndCondition termAndCondition);
        IDataResult<TermAndCondition> Get(); 
    }
}