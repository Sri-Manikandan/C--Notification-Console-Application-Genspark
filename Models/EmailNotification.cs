using System;

namespace Sns.Models{
    internal class EmailNotification : Notification{
        public EmailNotification(DateTime sentTime, string message, string userName)
            : base(sentTime, message, NotificationType.Email){
            Console.WriteLine("Email Sent to " + userName + " : " + Message);
        }
    }
}
