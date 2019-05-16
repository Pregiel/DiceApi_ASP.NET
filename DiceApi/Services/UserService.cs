using DiceApi.Entities;
using DiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(User user, string password);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task Update(User userParam, string password = null);
        Task Delete(int id);
    }
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            if (user == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            if (!PasswordHelpers.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            return user;
        }

        public async Task<User> Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.PasswordNull);

            if (await _context.Users.AnyAsync(x => x.Username == user.Username))
                throw new ApplicationException(Properties.resultMessages.UsernameExists);

            byte[] passwordHash, passwordSalt;
            PasswordHelpers.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task Update(User userParam, string password = null)
        {
            var user = await _context.Users.FindAsync(userParam.Id);

            if (user == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            if (userParam.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(x => x.Username == userParam.Username))
                    throw new ApplicationException(Properties.resultMessages.UsernameExists);
            }

            user.Username = userParam.Username;
            
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                PasswordHelpers.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
