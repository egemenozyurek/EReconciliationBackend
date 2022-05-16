using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Castle.DynamicProxy;
using Core.Aspects.Performance;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspect.Performance
{
    public class PerformanceAspect : MethodInterception
    {
        private int _interval;
        private Stopwatch _stopwatch;

        public PerformanceAspect(int interval)
        {
            _interval = interval;
            _stopwatch = ServiceTool.ServiceProvider.GetService<Stopwatch>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Start();
        }

        protected override void OnAfter(IInvocation invocation)
        {
            if (_stopwatch.Elapsed.TotalSeconds > _interval)
            {
                string body = $"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name} --> {_stopwatch.Elapsed.TotalSeconds}";
                SendConfirmEmail(body);
            }
            _stopwatch.Reset();
        }

        void SendConfirmEmail(string body)
        {
            string subject = "Performans Maili";

            SendMailDto sendMailDto = new SendMailDto()
            {
                Email = "emutabakat09@gmail.com",
                Password = "eMutabakat0900",
                Port = 587,
                SMTP = "smtp.gmail.com",
                SSL = true,
                email = "emutabakat09@gmail.com",
                subject = subject,
                body = body
            };

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(sendMailDto.Email);
                mail.To.Add(sendMailDto.email);
                mail.Subject = sendMailDto.subject;
                mail.Body = sendMailDto.body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(sendMailDto.SMTP))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(sendMailDto.Email, sendMailDto.Password);
                    smtp.EnableSsl = sendMailDto.SSL;
                    smtp.Send(mail);
                }
            }
        }
    }
}