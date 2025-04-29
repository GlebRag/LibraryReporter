﻿using LibraryReporter.Data.Models;
using LibraryReporter.Data;
using Enums.Users;
using Microsoft.EntityFrameworkCore;
using LibraryReporter.Data.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;

namespace LibraryReporter.Data.Repositories
{
    public interface IUserRepositryReal : IUserRepositry<UserData>
    {
        string GetAvatarUrl(int userId);
        bool IsAdminExist();
        bool IsLoginAndPasswordIsCorrect(string login, string password);
        bool IsLoginUniq(string name);
        UserData? Login(string login, string password);
        void Register(string login, string password, Role role = Role.User);
        void UpdateAvatarUrl(int? userid, string avatarUrl);
        void UpdateLocal(int? userId, Language language);
        void UpdateRole(int userId, Role role);
    }

    public class UserRepository : BaseRepository<UserData>, IUserRepositryReal
    {
        public UserRepository(WebDbContext webDbContext) : base(webDbContext)
        {
        }

        public override void Add(UserData data)
        {
            throw new NotImplementedException("User method Register to create a new User");
        }

        public string GetAvatarUrl(int userId)
        {
            return _dbSet.First(x => x.Id == userId).AvatarUrl;
        }

        public bool IsAdminExist()
        {
            return _dbSet.Any(x => x.Role.HasFlag(Role.Admin));
        }

        public bool IsLoginAndPasswordIsCorrect(string login, string password)
        {
            return !_dbSet.Any(x => x.Login == login) && !_dbSet.Any(x => x.Password == password);
        }

        public bool IsLoginUniq(string login)
        {
            return !_dbSet.Any(x => x.Login == login);
        }

        public UserData? Login(string login, string password)
        {

            var brokenPassword = BrokePassword(password);

            return _dbSet.FirstOrDefault(x => x.Login == login && x.Password == brokenPassword);
        }

        public void Register(string login, string password, Role role = Role.User)
        {
            var user = new UserData
            {
                Login = login,
                Password = BrokePassword(password),
                Role = role,
                AvatarUrl = "/images/avatars/default.png",
                Language = Language.Ru
            };

            _dbSet.Add(user);
            _webDbContext.SaveChanges();
        }

        public void UpdateAvatarUrl(int? userId, string avatarUrl)
        {
            var user = _dbSet.First(x => x.Id == userId);
            user.AvatarUrl = avatarUrl;
            _webDbContext.SaveChanges();
        }

        public void UpdateLocal(int? userId, Language language)
        {
            var user = _dbSet.First(x => x.Id == userId);

            user.Language = language;

            _webDbContext.SaveChanges();
        }

        public void UpdateRole(int userId, Role role)
        {
            var user = _dbSet.First(x => x.Id == userId);
            user.Role = role;
            _webDbContext.SaveChanges();
        }


        private string BrokePassword(string originalPassword)
        {
            // jaaaack
            // jacke
            // jack
            var brokenPassword = originalPassword.Replace("a", "");

            // jck
            return brokenPassword;
        }
    }
}
