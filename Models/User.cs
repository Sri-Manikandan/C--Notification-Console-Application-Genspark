using System;

namespace Sns.Models{
    internal class User{
        public string Name {get;set;}
        public string Email {get;set;}
        public string PhoneNumber {get;set;}
        public bool IsActive {get;set;}

        public User(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            IsActive = true;
        }

        public override string ToString(){
            return "User : " + Name + "\nEmail : " + Email + "\nPhone Number : " + PhoneNumber + "\nActive : " + IsActive;
        }
    }
}
