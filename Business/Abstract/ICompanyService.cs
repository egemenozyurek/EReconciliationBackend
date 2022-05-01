using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface ICompanyService
    {
        IResult Add(Company company);
        IResult Update(Company company);
        IDataResult<Company> GetById(int id);
        IResult AddCompanyAndUserCompany(CompanyDto companyDto);
        IDataResult<List<Company>> GetList();
        IDataResult<List<Company>> GetListByUserId(int userId);
        IDataResult<UserCompany> GetCompany(int userId);
        IResult CompanyExists(Company company);
        IResult UserCompanyAdd(int userId, int companyId);        
    }
}