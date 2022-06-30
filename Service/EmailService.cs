using Microsoft.AspNetCore.Components;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace MESystem.Data
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmail(string path)
        {
            _logger.LogInformation(DateTimeOffset.Now.ToString());

           // await SendingEmail(path);
        }

        public static async Task SendingEmail(string path, string shipmentId)
        {
            try
            {
                //MailAddress to = new MailAddress("sebastian.wermeling@friwo.com");
                //MailAddress to = new("it.vn@friwo.com");
                //MailAddress from = new("it.vn@friwo.com", "FRIWO Planning Notification");

                MailMessage message = new MailMessage();
                message.From = new("hello@friwo.com", "FRIWO Planning Notification");

                message.To.Add(new MailAddress("danny.vu@friwo.com"));
                message.To.Add(new MailAddress("johnny.do@friwo.com"));
                message.To.Add(new MailAddress("shelly.nguyen@friwo.com"));
                message.To.Add(new MailAddress("hendrik.brendel@friwo.com"));
                message.To.Add(new MailAddress("may.nguyen@friwo.com"));
                message.To.Add(new MailAddress("evy.nguyen@friwo.com"));
                message.To.Add(new MailAddress("alice.hoang@friwo.com"));
                message.To.Add(new MailAddress("phu.chac@friwo.com"));

                message.CC.Add(new MailAddress("it.vn@friwo.com"));
                message.CC.Add(new MailAddress("artur.petrosjan@friwo.com"));
                message.CC.Add(new MailAddress("hendrik.brendel@friwo.com"));

                message.Subject = "FRIWO Planning Notification";
                message.Body = $"Dear all, \n{DateTimeOffset.Now} \nNew shipments ({shipmentId}), was arrived please check!!!";

                //MailAddress copy = new("it.vn@friwo.com");
               

                SmtpClient client = new("10.100.10.60", 25)
                {
                    Host = "10.100.10.60"
                };

                Attachment data = new Attachment(path);
                // Add time stamp information for the file.
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(path);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(path);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(path);
                // Add the file attachment to this email message.
                message.Attachments.Add(data);

      
                    await client.SendMailAsync(message);
                }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}