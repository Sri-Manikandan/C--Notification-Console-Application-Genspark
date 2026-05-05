using System;

namespace Sns.Interfaces{
    internal interface INotificationService{
        public void sendMessage(string Message);
    }
}