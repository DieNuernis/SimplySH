using Microsoft.EntityFrameworkCore;
using System;
using SimplySH.Models.SSH;

namespace SimplySH.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {
        }

        public DbSet<SSHConnection> SSHConnections { get; set; }
    }
}