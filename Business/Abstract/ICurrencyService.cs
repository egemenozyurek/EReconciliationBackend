using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface ICurrencyService
    {
        IDataResult<List<Currency>> GetList();
    }
}