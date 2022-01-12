using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CandleInTheWind.API.Helpers
{
    public class MailSender
    {
        private readonly SmtpClient _smtp;
        private readonly string _senderAddress;

        public MailSender(IConfiguration config)
        {
            var mailSettings = config.GetSection("MailSettings");
            _senderAddress = mailSettings.GetValue<string>("Address");
            _smtp = new SmtpClient()
            {
                Host = mailSettings.GetValue<string>("Host"),
                Port = mailSettings.GetValue<int>("Port"),
                Credentials = new NetworkCredential()
                {
                    UserName = _senderAddress,
                    Password = mailSettings.GetValue<string>("Password")
                },
                EnableSsl = true,
            };
        }

        public async Task<bool> SendMailAsync(string body, string receiverAddress)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(_senderAddress),
                Subject = "Link reset mật khẩu - CandleInTheWind shop",
                Body = body,
                IsBodyHtml = true,
            };
            mail.To.Add(receiverAddress);

            try
            {
                await _smtp.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
