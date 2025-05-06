using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MSU_BARODA.Services
{
    public class EmailServiceAdmin
    {
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailServiceAdmin(IConfiguration configuration)
        {
            _senderEmail = "meetsachdev1245@gmail.com"; // Your email
            _senderPassword = "ilotmwhrussxpkvo"; // Your App Password
        }

        public async Task SendBulkEmailAsync(List<string> recipientEmails, string subject, string body, string attachmentName, byte[] attachmentData)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    client.EnableSsl = true;

                    for (int i = 0; i < recipientEmails.Count; i += 50)
                    {
                        var batch = recipientEmails.GetRange(i, Math.Min(50, recipientEmails.Count - i));

                        using (var mailMessage = new MailMessage())
                        {
                            mailMessage.From = new MailAddress(_senderEmail);
                            mailMessage.Subject = subject;
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = false;

                            foreach (var email in batch)
                                mailMessage.To.Add(email);

                            if (attachmentData != null)
                                mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachmentData), attachmentName));

                            await client.SendMailAsync(mailMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
