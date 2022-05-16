using Business.Abstract;
using Business.BusinessAspects;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class TermAndConditionManager : ITermAndConditionService
    {
        private readonly ITermAndConditionDal _termAndConditionDal;

        public TermAndConditionManager(ITermAndConditionDal termAndConditionDal)
        {
            _termAndConditionDal = termAndConditionDal;
        }

        public IDataResult<TermAndCondition> Get()
        {
            return new SuccessDataResult<TermAndCondition>(_termAndConditionDal.GetList().FirstOrDefault());
        }

        [SecuredOperation("Admin")]
        public IResult Update(TermAndCondition termAndCondition)
        {
            var result = _termAndConditionDal.GetList().FirstOrDefault();
            if (result is not null)
            {
                result.Description = termAndCondition.Description;
                _termAndConditionDal.Update(termAndCondition);
            }
            else
            {
                _termAndConditionDal.Add(termAndCondition);
            }

            return new SuccessResult(Messages.UpdateTermsAndConditions);
        }
    }
}