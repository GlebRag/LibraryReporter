﻿using Enums.Users;
using LibraryReporter.Data.Models;
using LibraryReporter.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryReporter.Data
{
    public class Seed
    {
        public void Fill(IServiceProvider service)
        {
            using var di = service.CreateScope();

            UserFill(di);
        }

        private void UserFill(IServiceScope di)
        {
            var userRepositry = di.ServiceProvider.GetRequiredService<IUserRepositryReal>();
            if (userRepositry.IsAdminExist())
            {
                return;
            }

            userRepositry.Register("admin", "admin", Role.Admin);
        }
    }
}
