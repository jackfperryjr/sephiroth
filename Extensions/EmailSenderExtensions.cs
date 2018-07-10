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
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Jenova thanks you for registering with us. Please follow this <a href='{HtmlEncoder.Default.Encode(link)}'>link</a> to complete your registration and login.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Reset password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        public static Task SendStatusUpdateAsync(this IEmailSender emailSender, string email, RequestStatus status)
        {
            string stringStatus = status.ToString();

            if (stringStatus == "Submitted")
            {
                return emailSender.SendEmailAsync(email, "Status update of your request",
                $"Your request has been {stringStatus.ToLower()} for Sephiroth to ask Jenova if it's okay.");
            }
            else 
            {
                return emailSender.SendEmailAsync(email, "Status update of your request",
                    $"Your request has been {stringStatus.ToLower()} by Sephiroth. Jenova said so.");
            }
        }
    }
}