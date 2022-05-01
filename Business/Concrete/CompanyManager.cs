using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transactions;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Concrete
{
    public class CompanyManager : ICompanyService
    {
        private readonly ICompanyDal _companyDal;
        private readonly IOperationClaimService _operationClaimService;
        private readonly IUserOperationClaimService _userOperationClaimService;
        private readonly IMailTemplateService _mailTemplateService;
        public CompanyManager(ICompanyDal companyDal, IOperationClaimService operationClaimService, IUserOperationClaimService userOperationClaimService, IMailTemplateService mailTemplateService)
        {
            _companyDal = companyDal;
            _operationClaimService = operationClaimService;
            _userOperationClaimService = userOperationClaimService;
            _mailTemplateService = mailTemplateService;
        }

        [ValidationAspect(typeof(CompanyValidator))]
        public IResult Add(Company company)
        {
            if (company.Name.Length > 10)
            {
                _companyDal.Add(company);
                return new SuccessResult(Messages.AddedCompany);
            }

            return new ErrorResult("Şirket adı en az 10 karakter olmalı.");
        }

        [ValidationAspect(typeof(CompanyValidator))]
        [TransactionScopeAspect]
        public IResult AddCompanyAndUserCompany(CompanyDto companyDto)
        {
            Company company = new Company()
            {
                Id = companyDto.Id,
                Name = companyDto.Name,
                TaxDepartment = companyDto.TaxDepartment,
                IdentityNumber = companyDto.IdentityNumber,
                Address = companyDto.Address,
                AddedAt = companyDto.AddedAt,
                IsActive = companyDto.IsActive
            };

            _companyDal.Add(company);
            _companyDal.UserCompanyAdd(companyDto.UserId, company.Id);

            var operationClaims = _operationClaimService.GetList().Data;
            foreach (var operationClaim in operationClaims)
            {
                if (operationClaim.Name != "Admin")
                {
                    UserOperationClaim userOperation = new UserOperationClaim()
                    {
                        CompanyId = company.Id,
                        AddedAt = DateTime.Now,
                        IsActive = true,
                        OperationClaimId = operationClaim.Id,
                        UserId = companyDto.UserId
                    };

                    _userOperationClaimService.Add(userOperation);
                }
            }
            // var mailTemplate = _mailTemplateService.GetByCompanyId(1).Data;
            // mailTemplate.Id = 0;
            // mailTemplate.Type = "Mutabakat";
            // mailTemplate.CompanyId = company.Id;
            // _mailTemplateService.Add(mailTemplate);

            return new SuccessResult(Messages.AddedCompany);
        }

        public IResult CompanyExists(Company company)
        {
            var result = _companyDal.Get(c => c.Name == company.Name && c.TaxDepartment == company.TaxDepartment && c.TaxIdNumber == company.TaxIdNumber && c.IdentityNumber == company.IdentityNumber);

            if (result is not null)
                return new ErrorResult(Messages.CompanyAlreadyExists);

            return new SuccessResult();
        }

        public IDataResult<Company> GetById(int id)
        {
            return new SuccessDataResult<Company>(_companyDal.Get(p => p.Id == id));
        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyDal.GetCompany(userId));
        }

        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetList());
        }

        public IDataResult<List<Company>> GetListByUserId(int userId)
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetListByUserId(userId));
        }

        public IResult Update(Company company)
        {
            _companyDal.Update(company);
            return new SuccessResult(Messages.UpdatedCompany);
        }

        public IResult UserCompanyAdd(int userId, int companyId)
        {
            _companyDal.UserCompanyAdd(userId, companyId);
            return new SuccessResult();
        }
    }
}