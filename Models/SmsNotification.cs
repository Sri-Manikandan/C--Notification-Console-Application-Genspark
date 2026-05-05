using System;

namespace Sns.Models{

    internal class SmsNotification : Notification{
        public SmsNotification(DateTime sentTime, string message, User user)
            : base(sentTime, message, NotificationType.SMS,user){
        }

        public override void Send(){
            Console.WriteLine( "SMS sent to " + Recipient.PhoneNumber + ": " + Message);
        }
    }
}
