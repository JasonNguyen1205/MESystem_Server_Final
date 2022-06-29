using Microsoft.AspNetCore.Components;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace MESystem.Data
{
    public class EmailService
    {
        [Inject]
        public static UploadFileService UploadFileService { get; set; }
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmail(string path)
        {
            _logger.LogInformation(DateTimeOffset.Now.ToString());

            await SendingEmail(path);
        }

        public static async Task SendingEmail(string path)
        {

            //MailAddress to = new MailAddress("sebastian.wermeling@friwo.com");
            MailAddress to = new("it.vn@friwo.com");
            MailAddress from = new("smith.tran@friwo.com", "FRIWO Planning Notification");

            MailMessage message = new(from, to);
            message.Subject = "FRIWO Planning Notification";
            message.Body = $"Dear all, \n{DateTimeOffset.Now} \n New shipments arrival please check!!!";

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

            try
            {
                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}