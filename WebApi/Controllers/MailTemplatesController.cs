using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAp.Controllers
{
    public class MailTemplatesController : ControllerBase
    {
        private readonly IMailTemplateService _mailTemplateService;

        public MailTemplatesController(IMailTemplateService mailTemplateService)
        {
            _mailTemplateService = mailTemplateService;
        }

        [HttpPost("add")]
        public IActionResult Add(MailTemplate mailTemplate)
        {
            var result = _mailTemplateService.Add(mailTemplate);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}