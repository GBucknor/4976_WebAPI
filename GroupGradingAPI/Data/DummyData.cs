using GroupGradingAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Data
{
    public class DummyData
    {
        public static async Task Initialize(GradingContext context,
                           UserManager<IdentityUser> userManager)
        {
            context.Database.EnsureCreated();

            string password = "P@$$w0rd";


            var ff = "1234";
            if (await userManager.FindByNameAsync(ff) == null)
            {
                var user = new Student
                {
                    FirstName = ff,
                    LastName = "xx",
                    Email = ff + "@np.com",
                    UserName = ff
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Student");
                }
            }
            ff = "admin";
            if (await userManager.FindByNameAsync(ff) == null)
            {
                var user = new Student
                {
                    FirstName = ff,
                    LastName = "xx",
                    Email = ff + "@np.com",
                    UserName = ff
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Teacher");
                }
            }
            ff = "2345";
            if (await userManager.FindByNameAsync(ff) == null)
            {
                var user = new Student
                {
                    FirstName = ff,
                    LastName = "xx",
                    Email = ff + "@np.com",
                    UserName = ff
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Student");
                }
            }
            ff = "3456";
            if (await userManager.FindByNameAsync(ff) == null)
            {
                var user = new Student
                {
                    FirstName = ff,
                    LastName = "xx",
                    Email = ff + "@np.com",
                    UserName = ff
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Student");
                }
            }
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine("Created" + i);
                var xx = "" + (i * 1111);
                if (await userManager.FindByNameAsync(xx) == null)
                {
                    var user = new Student
                    {
                        FirstName = xx,
                        LastName = "xx",
                        Email = xx + "@np.com",
                        UserName = xx
                    };
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Student");
                    }
                }
            }
          




        }
    }

}
