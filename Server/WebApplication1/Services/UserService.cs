using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User GetByUserName(string userName);
        User Create(User user, string password);
        void Update(User user, string password);
        void Delete(int id);
        IEnumerable<ConversationUser> GetConversationUsers(int id);
    }

    public class UserService : IUserService
    {
        private WebApplication1Context _context;

        public UserService(WebApplication1Context context)
        {
            _context = context;
        }

        // Helper methods

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        public User Authenticate(string username, string password)
        {
            // Check
            if (string.IsNullOrEmpty(username))
                throw new Exception("Username is required");

            if (string.IsNullOrEmpty(password))
                throw new Exception("Password is required");

            // Find
            var user = _context.Users.SingleOrDefault(x => x.UserName == username);
            if (user == null)
                throw new Exception("User not found");

            // Verify
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("UserName or password is incorrect");

            // Ok
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                throw new Exception("User not found");

            return user;
        }

        public User GetByUserName(string userName)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserName == userName);
            if (user == null)
                throw new Exception("User not found");

            return user;
        }

        public User Create(User user, string password)
        {
            // Validate
            if (string.IsNullOrEmpty(user.UserName))
                throw new Exception("UserName is required");

            if (string.IsNullOrEmpty(password))
                throw new Exception("Password is required");

            // UserName is already taken
            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new Exception("Username \"" + user.UserName + "\" is already taken");

            // Hash password
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Add
            _context.Users.Add(user);
            _context.SaveChanges();
            
            // Ok
            return user;
        }

        public void Update(User user, string password)
        {
            // Find
            var updatedUser = _context.Users.Find(user.Id);   
            if (updatedUser == null)
                throw new Exception("User not found");

            // Update UserName
            if (!string.IsNullOrEmpty(user.UserName))
            {
                // UserName is already taken
                if (_context.Users.Any(x => x.UserName == user.UserName))
                    throw new Exception("Username is already taken");

                updatedUser.UserName = user.UserName;
            }

            // Update FullName
            if (!string.IsNullOrEmpty(user.FullName))
                updatedUser.FullName = user.FullName;

            // Update Avatar
            if (!string.IsNullOrEmpty(user.Avatar))
                updatedUser.Avatar = user.Avatar;

            // Update Password
            if (!string.IsNullOrEmpty(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                if (updatedUser.PasswordHash == passwordHash && updatedUser.PasswordSalt == passwordSalt)
                    throw new Exception("Password is the same as yours");

                updatedUser.PasswordHash = passwordHash;
                updatedUser.PasswordSalt = passwordSalt;
            }

            // Update Role
            if (!string.IsNullOrEmpty(user.Role))
            {
                if (!Role.AvailableRoles.Contains(user.Role))
                    throw new Exception("Role is not supported");

                updatedUser.Role = user.Role;
            }

            // Update
            _context.Users.Update(updatedUser);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            // Find
            var user = _context.Users.Find(id);
            if (user == null)
                throw new Exception("User not found");

            // Delete
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public IEnumerable<ConversationUser> GetConversationUsers(int id)
        {
            // Find
            var user = _context.Users.Find(id);
            if (user == null)
                throw new Exception("User not found");

            return user.ConversationUsers;
        }
    }
}
