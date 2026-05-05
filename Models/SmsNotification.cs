using System;

namespace Sns.Models{
    internal class SmsNotification : Notification{
        public SmsNotification(DateTime sentTime, string message, string userName)
            : base(sentTime, message, NotificationType.SMS){
            Console.WriteLine("SMS Sent to " + userName + " : " + Message);
        }
    }
}
