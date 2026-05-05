using System;
using System.Collections.Generic;
using Sns.Models;
using Sns.Interfaces;
using Sns.Repositories;

namespace Sns.Services{
    internal class NotificationService : INotificationService{
        private UserRepository _userRepository = new UserRepository();

        public User AddUser(string name, string email, string phoneNumber){
            User user = new User(name, email, phoneNumber);
            return _userRepository.Add(user);
        }

        public List<User> GetAllUsers(){
            return _userRepository.GetAll();
        }

        public User? GetUser(string email){
            return _userRepository[email];
        }

        public User? UpdateUser(string email, string name, string phoneNumber){
            User? existing = _userRepository.Get(email);
            if(existing == null) return null;
            User updated = new User(name, email, phoneNumber);
            return _userRepository.Update(email, updated);
        }

        public User? DeleteUser(string email){
            return _userRepository.Delete(email);
        }

        public void sendMessage(string message){
            List<User> users = _userRepository.GetAll();
            if(users.Count == 0){
                Console.WriteLine("No users to notify.");
                return;
            }
            foreach(var user in users){
                new EmailNotification(DateTime.Now, message, user.Name);
                new SmsNotification(DateTime.Now, message, user.Name);
            }
        }
    }
}
