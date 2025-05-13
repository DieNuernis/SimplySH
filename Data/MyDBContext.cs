using Microsoft.EntityFrameworkCore;
using System;
using SimplySH.Models.SSH;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SimplySH.Models.Auth;

namespace SimplySH.Data
{
    public class MyDBContext : IdentityDbContext<ApplicationUser>
    {
        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {
        }

        public DbSet<SSHConnection> SSHConnections { get; set; }
    }
}