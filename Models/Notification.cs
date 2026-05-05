using System;

namespace Sns.Models{
    public enum NotificationType{
        Email,
        SMS
    }

    public abstract class Notification{
        public DateTime SentTime {get ; set;}
        public string Message {get;set;}
        public NotificationType NotificationType { get; set; }
        public User Recipient {get;set;}


        protected Notification(DateTime sentTime, string message, NotificationType notificationType, User recipient){
            SentTime=sentTime;
            Message=message;
            NotificationType=notificationType;
            Recipient=recipient;
        }
        public override string ToString(){
            return "Notification : " + Message + "\nSent Time : " + SentTime + "\nNotification Type : " + NotificationType + "\nRecipient : " + Recipient.UserName;
        }

        public abstract void Send();
    }
}
