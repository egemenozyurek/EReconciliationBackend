using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
    public class SendMailDto : IDto
    {
        public MailParameter MailParameter {get; set;}
        public string Email {get; set;}
        public string Subject { get; set;}
        public string Body {get; set;}
    }
}