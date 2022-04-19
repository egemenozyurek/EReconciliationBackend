using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class CompanyManager : ICompanyService
    {
        private readonly ICompanyDal _companyDal;

        public CompanyManager(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }

        public IResult Add(Company company)
        {
            if(company.Name.Length > 10){
                _companyDal.Add(company);
                return new SuccessResult(Messages.AddedCompany);
            }

            return new ErrorResult("Şirket adı en az 10 karakter olmalı.");
        }

        public IResult CompanyExists(Company company)
        {
            throw new NotImplementedException();
        }

        public IDataResult<Company> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetList());
        }

        public IDataResult<List<Company>> GetListByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public IResult Update(Company company)
        {
            throw new NotImplementedException();
        }

        public IResult UserCompanyAdd(int userId, int companyId)
        {
            throw new NotImplementedException();
        }
    }
}