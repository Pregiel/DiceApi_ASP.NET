using DiceApi.Entities;
using DiceApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceApi.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User Create(User user, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Update(User userParam, string password = null);
        void Delete(int id);
    }
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            if (!PasswordHelpers.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            return user;
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.PasswordNull);

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ApplicationException(Properties.resultMessages.UsernameNull);

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new ApplicationException(Properties.resultMessages.UsernameExists);

            byte[] passwordHash, passwordSalt;
            PasswordHelpers.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.SingleOrDefault(x => x.Id == id);
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == userParam.Id);

            if (user == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            if (userParam.Username != user.Username)
            {
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new ApplicationException(Properties.resultMessages.UsernameExists);
            }

            user.Username = userParam.Username;
            
            if (!string.IsNullOrWhiteSpace(password))
            {
                PasswordHelpers.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
