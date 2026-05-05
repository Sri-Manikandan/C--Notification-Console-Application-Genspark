using System;

namespace Sns.Models{
    internal class EmailNotification : Notification{
        public EmailNotification(DateTime sentTime, string message, User user)
            : base(sentTime, message, NotificationType.Email, user){
        }

        public override void Send(){
            Console.WriteLine( "Email sent to " + Recipient.Email + ": " + Message);
        }
    }
}
