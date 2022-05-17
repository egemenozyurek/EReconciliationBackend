using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class TermsAndConditionController : ControllerBase
    {
        private readonly ITermAndConditionService _termsAndConditionService;

        public TermsAndConditionController(ITermAndConditionService termsAndConditionService)
        {
            _termsAndConditionService = termsAndConditionService;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            var result = _termsAndConditionService.Get();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("update")]
        public IActionResult Update(TermAndCondition termsandCondition)
        {
            var result = _termsAndConditionService.Update(termsandCondition);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}