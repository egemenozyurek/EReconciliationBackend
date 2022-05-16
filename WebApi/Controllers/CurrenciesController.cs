using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{   
    [ApiController]
    [Route("[controller]")]
    public class CurrenciesController : ControllerBase
    {   
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            var result = _currencyService.GetList();
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
    }
}