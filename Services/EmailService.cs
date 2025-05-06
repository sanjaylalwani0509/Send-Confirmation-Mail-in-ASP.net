using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MSU_BARODA.Services
{
    public class EmailService
    {
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(IConfiguration configuration)
        {
            _senderEmail = configuration["EmailSettings:SenderEmail"];
            _senderPassword = configuration["EmailSettings:SenderPassword"];
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body, string attachmentName, byte[] attachmentData)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(recipientEmail);

                    if (attachmentData != null)
                    {
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachmentData), attachmentName));
                    }

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}