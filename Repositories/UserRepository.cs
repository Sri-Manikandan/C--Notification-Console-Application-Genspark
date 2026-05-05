using System.Collections.Generic;
using Sns.Models;
using Sns.Interfaces;

namespace Sns.Repositories{
    internal class UserRepository : IRepository<User, string>{
        private Dictionary<string, User> _users = new Dictionary<string, User>();

        public User? this[string email]{
            get { return _users.ContainsKey(email) ? _users[email] : null; }
        }

        public User Add(User user){
            _users[user.Email] = user;
            return user;
        }

        public List<User> GetAll(){
            return new List<User>(_users.Values);
        }

        public User? Get(string email){
            return _users.ContainsKey(email) ? _users[email] : null;
        }

        public User? Update(string email, User updatedUser){
            if(!_users.ContainsKey(email)) return null;
            _users[email] = updatedUser;
            return updatedUser;
        }

        public User? Delete(string email){
            if(!_users.ContainsKey(email)) return null;
            User user = _users[email];
            user.IsActive = false;
            _users.Remove(email);
            return user;
        }
    }
}
