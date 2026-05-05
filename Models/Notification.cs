using System;

namespace Sns.Models{
    public enum NotificationType{
        Email,
        SMS
    }
    public class Notification{
        public DateTime SentTime {get;set;}
        public string Message {get;set;}
        public NotificationType NotificationType {get;set;}
        public Notification(DateTime sentTime, string message, NotificationType notificationType){
            SentTime = sentTime;
            Message = message;
            this.NotificationType = notificationType;
        }
        public override string ToString(){
            return "Notification : " + Message + "\nSent Time : " + SentTime + "\nNotification Type : " + NotificationType;
        }
    }
}
