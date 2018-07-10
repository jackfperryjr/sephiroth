using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Sephiroth.Services;
using Sephiroth.Models;

namespace Sephiroth.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Please confirm your email",
                $"<br/><br/>Dear person,<br/><br/>This email is being sent because you registered as a user in my domain. If this was a mistake, you can ignore it. But be wary; someone's using your email address.<br/><br/>If it was you, Jenova thanks you for registering with me. Please follow this <a href='{HtmlEncoder.Default.Encode(link)}'>link</a> to complete your registration and login.<br/><br/><br/>Yours truly,<br/><br/><em>Sephiroth</em>");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Reset password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        public static Task SendStatusUpdateAsync(this IEmailSender emailSender, string email, RequestStatus status, string name, string dateOfRequest, string reason)
        {
            string stringStatus = status.ToString();

            if (stringStatus == "Submitted")
            {
                return emailSender.SendEmailAsync(email, "You made a request",
                $"<br/><br/>Dear {name},<br/><br/>Thank you for submitting a request for time off. Your time is important to me. Ha! Just kidding. But your request has been {stringStatus.ToLower()} to me for consideration. I'll ask Jenova if it's okay.<br/><br/><strong>Details</strong>:<br/>Name: {name}<br/>Requested date: {dateOfRequest}.<br/>Requested reason: {reason}.<br/><br/><br/>Yours truly,<br/><br/><em>Sephiroth</em>");
            }
            else 
            {
                return emailSender.SendEmailAsync(email, "Status update of your request",
                $"<br/><br/>Dear {name},<br/><br/>This email is being sent because you made a request for time off. After careful consideration your request has been <strong>{stringStatus.ToLower()}</strong> by Jenova. She's my mother.<br/><br/><strong>Details</strong>:<br/>Name: {name}<br/>Requested date: {dateOfRequest}.<br/>Requested reason: {reason}.<br/><br/><br/>Yours truly,<br/><br/><em>Sephiroth</em>");
            }
        }
    }
}