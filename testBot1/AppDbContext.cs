using System;
using System.Linq;
using System.Data.Entity;

namespace testBot1
{
    class AppDbContext : DbContext
    {
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext() : base("DbConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        public User GetUserByTelegramId(int telegramId)
        {
            return Users.FirstOrDefault(e => e.TelegramUserId == telegramId);
        }

    }
}
